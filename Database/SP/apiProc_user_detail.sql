USE [WePayNepal]
GO

/****** Object:  StoredProcedure [dbo].[apiProc_user_detail]    Script Date: 6/5/2020 12:57:17 PM ******/
DROP PROCEDURE [dbo].[apiProc_user_detail]
GO

/****** Object:  StoredProcedure [dbo].[apiProc_user_detail]    Script Date: 6/5/2020 12:57:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE                 PROCEDURE [dbo].[apiProc_user_detail] 
	@flag  char(3) = null,
	@user_name varchar(50) =  null,
	@user_old_Password varchar(50) =  null,
	@user_new_Password varchar(50) = null,
	@user_confirm_password varchar(50) = null,
	@user_new_mPin int =  null,
	@new_user_mobileNumber varchar(50) =  null,
	@new_user_email varchar(50) =  null,
	@new_user_full_name varchar(50) =  null,
	@user_confirm_mpin varchar(50) =  null,
	@action_ip_address varchar(15) =  null,
	@user_verification_code_send varchar(100) = null


AS
BEGIN
declare @user_id int, @user_status char(3), @gen_verification_code nvarchar(20),@id int,@new_card_no varchar(20),@user_email varchar(100), @tbl_user_name varchar(100), @user_verification_code_pr varchar(100)
declare @user_verification_status varchar(100), @user_email_address varchar(100), @user_full_name varchar(100), @send_datetime datetime 

	set @new_card_no = '1000'+cast(convert(numeric(12,0),rand() * 899999999999) + 100000000000 as varchar)
	set @gen_verification_code = dbo.[func_generate_verify_code](6) 
	--set @gen_verification_code = dbo.[func_generate_random_number] ((isnull(@new_user_mobileNumber, @user_name)) + cast(GEtDate() as varchar))

	--change Password
	if @flag = 'cp'
	begin

	--first check if user exists

		if not exists(select  'x' from tbl_user_detail where ([user_name] = @user_name or user_email = @user_name) and isnull(status,'n') <> 'n')
		begin
			select '104' Code, 'User Not found!' message, null id
			return
		end
		else
		--added for userid null bug fix
			begin
			select @user_id = user_id from tbl_user_detail where  (user_name = @user_name or user_email = @user_name) and isnull(status,'n') <> 'n'
			end

		if exists (SELECT 'x'
				FROM tbl_user_detail WITH (NOLOCK)
				WHERE (user_name = @user_name  or user_email  = @user_name)
				AND pwdcompare(@user_old_Password, password) <> 1)
					begin
						select '110' Code, 'User Old Password didn''t match' message, null id
						return
					end
		else if(len(@user_new_Password) < 8)
			begin
				Select '111' code, 'New Password must be more than 10 characters' message, null id
				return
			end
		else if(@user_new_Password <> @user_confirm_password)
		begin
			select '112' code, 'User new Password and Confirm password didn''t match' message, null id
			return
		end
		
		else
		begin
			update tbl_user_detail set password = isnull(PWDENCRYPT(@user_new_Password),password)
			where (user_name= @user_name or user_email = @user_name)
			select '0' code, 'Password succesfully Updated' message, @user_id id
			return

		end
	end

	--change mPin
	if @flag = 'mp'
	begin
	--first check if user exists

		if not exists(select  'x' from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name and isnull(status,'n') <> 'n' and pwdcompare(@user_old_Password, password) = 1)
		begin
			select '104' Code, 'User Not found!' message, null id
			return
		end
		else
		begin
		--added for userid null bug fix
		select @user_id = user_id from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name and isnull(status,'n') <> 'n'
		update tbl_user_detail set m_pin = PWDENCRYPT(@user_new_mPin) where user_mobile_no = @user_name or user_email = @user_name
		select '0' code, 'User MPin updated succesfully' message , @user_id id
		return
		end
	end

	--forgot pwd  send verification code
	if @flag = 'fpv'
	begin
		if not exists(select 'x' from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name)
		begin
			select '104' code, 'User Not Found!' message, null id
			return
		end
		else
		begin
			select @user_name = user_name, @user_email = user_email, @user_id = user_id,@tbl_user_name = user_name,@new_user_mobileNumber =  user_mobile_no,@new_user_full_name= full_name from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name

			insert into tbl_verification_sent (UserId, username, Mobile_Number,Email_address,full_Name,verification_code,generate_date_time)
			values(@user_id, @tbl_user_name,@new_user_mobileNumber, @user_email, @new_user_full_name, @gen_verification_code, GETDATE() )

			exec sproc_email_request @flag = 'd',@full_name = @new_user_full_name, @agent_verification_code =@gen_verification_code, @email_id = @user_email 
			update tbl_verification_sent set verification_Status = 'Sent',
					send_date_time=getdate() 
			where (Mobile_Number	= @new_user_mobileNumber 
			or Email_address		= @user_email) 
			and verification_code	= @gen_verification_code

			select '0' COde, 'Succesfully Sent Verification Code' message, @user_id id
			return

		end
	end

	--forgot pwd 
	if @flag = 'fp'
	begin
		if not exists(select 'x' from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name)
		begin
			select '104' code, 'User Not Found!' message, null id
			return
		end
		else
		begin
			select top 1 @user_verification_code_pr = verification_code, 
						@user_verification_status = verification_Status ,
						@user_email_address = Email_address,
						@user_full_name = full_Name,
						@send_datetime=send_date_time
			from tbl_verification_sent where vid =(select max(vid) from tbl_verification_sent where   Mobile_Number = @user_name or Email_address = @user_name)

		end


		if @user_verification_status  is null or ISNULL(@user_verification_status,'Sent') = trim('Verified')
		begin
		--- verifcation checked chase, if user is already verified we drop the temp table
			select '01' Code, 'Something went wrong please try again from the beginning' MEssage, null id
			return
		end
		
		if not exists (select 'x' from tbl_verification_sent where Mobile_Number =@user_name or Email_address  = @user_name and verification_code = @user_verification_code_send)
		begin
			select '1' code, 'Verification User not Found' message ,  null id
			return
		end
		
		if (@user_verification_code_pr = @user_verification_code_send)
		begin
		--	select @user_name = user_name from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name

		--	if @user_new_Password <> @user_confirm_password
		--	begin
		--		select '112' code, 'User new Password and Confirm password didn''t match' message, null id
		--		return
		--	end
		--	else
		--	begin
		--		update tbl_user_detail set password = PWDENCRYPT(@user_new_Password) where user_name = @user_name
				select '0' code, 'Code verified succesfully' message, null id
				return
			--end
		end
		else
		begin
			select '1' code, 'Something went Wrong, please resend Code again' message, null id
			return
		end
	end

	--get user details
	if @flag = 'gd'
	begin
		if not exists(select 'x' from tbl_user_detail where user_name = @user_name)
		begin
			select '104' code, 'User not Found' message, null id
			return
		end
		else
		begin
			select
			[user_id],
			full_name as [user_full_name],
			agent_id as [user_agent_id],
			user_email as [user_email_id],
			user_mobile_no as [user_mobile_no],
			[status] as [user_status],
			is_login_enabled as[is_user_login_enabled]
			from tbl_user_detail where user_name = @user_name
		end
	end

	--set password
	if @flag = 'np'
	begin
		--if @user_name is null
		--begin
		--	select '105' code, 'User name is required' message, null id
		--	return
		--end
		--else if not exists(select 'x' from tbl_user_detail where user_email = @user_name or user_mobile_no = @user_name)
		--begin
		--		select '104' code, 'User Not Found' message, null id
		--		return
		--end
		--else if(@user_new_Password <> @user_confirm_password)
		--begin
		--	select '112' code, 'Password and Confirm Password didn''t match' message, null id
		--	return
		--end
		--else
		--begin
		--	update tbl_user_detail set password = PWDEncrypt(@user_new_Password) where user_email = @user_name or user_mobile_no = @user_name
		--	select '0' code, 'Password set succesfully' message, null id
		--end

		if not exists(select 'x' from tbl_User_registration where Agent_verification_Status = 'Verified' and Agent_Mobile_Number = @user_name or Agent_Email_address =@user_name)
		begin
			select '1' code, 'Agent Not Verified' message, null id
			return
		end
		else if len(@user_new_Password) < 8
		begin
			select '1' code, 'Password must be more than 8 characters' message, null id
			return
		end
		else if (@user_new_Password <> @user_confirm_password)
		begin
			Select '1' code, 'Password and Confirm Password doesn''t match' message, null id
			return
		end
		else
		begin


		update tbl_user_detail set password  = PWDENCRYPT(@user_new_Password) where user_mobile_no = @user_name or user_email = @user_name

			--if exists(select 'x' from tbl_user_detail where user_mobile_no = @user_name or user_email = @user_name)
			--begin
			--	select '1' code, 'User already exists' message, null id
			--	return
			--end


			--set @user_name = 'User' + @new_user_mobileNumber

			--insert into tbl_agent_detail (agent_operation_type, agent_commission_id, agent_email_address, agent_mobile_no, full_name, agent_status,agent_type, kyc_status, created_by,created_local_date,created_UTC_date, created_nepali_date)
			--values('I',1,@new_user_email, @new_user_mobileNumber, @new_user_full_name,'Y' ,'WalletUser','N','System',GETDATE(), GETUTCDATE(),dbo.func_get_nepali_date(default))

			--set @id = SCOPE_IDENTITY()
			----insert into user detail
			--insert into tbl_user_detail(agent_id, user_name,password,user_email, user_mobile_no, full_name,status, is_login_enabled,created_by, created_ip, created_local_date, created_UTC_date, created_nepali_date,usr_type, usr_type_id,role_id )
			--values(@id,@user_name,PWDENCRYPT(@user_new_Password),@new_user_email, @new_user_mobileNumber, @new_user_full_name,'Y', 'Y','System',@action_ip_address, GETDATE(), GETUTCDATE(), dbo.func_get_nepali_date(default),'WalletUser','6','6')

			--set @user_ID = SCOPE_IDENTITY()
			--insert into tbl_kyc_documents (agent_id, KYC_Verified, created_by, created_local_date, created_UTC_date, created_nepali_date)
			--values(@id, 'Pending','System',GEtDATE(), GETUTCDATE(), dbo.func_get_nepali_date(default))
			
			--insert into tbl_agent_card_management(agent_id, card_no, user_name, user_id,card_type,card_issued_date, card_expiry_date,created_by,created_local_date,created_utc_date,created_nepali_date)
			--values(@id,@new_card_no,@user_name, @user_ID, '1', GETDATE(), DATEADD(YEAR, 4, GETDATE()),'System',GETDATE(), GETUTCDATE(), dbo.func_get_nepali_date(default))

			--exec sproc_email_request @flag = 'dr',@full_name = @new_user_full_name, @email_id = @new_user_email --,@password = @encrypted_password 

			select '0' code, 'User Registration Successfull' message, @user_id id
			return
		end

	end

	--set mpin
	if @flag = 'smp'
	begin
		if @user_name is null
		begin
			select '105' code, 'User name is required' message, null id
			return
		end
		else if not exists(select 'x' from tbl_user_detail where user_email = @user_name or user_mobile_no = @user_name)
		begin
				select '104' code, 'User Not Found' message, null id
				return
		end
		else if @user_new_mPin is null
		begin
			select '1' code, 'New Mpin is required' message , null id
			return
		end
		else if @user_confirm_mpin is null
		begin
			select '1' code, 'Confirm Mpin is required', null id
			return
		end
		else if(@user_new_mPin <> @user_confirm_mpin)
		begin
			select '112' code, 'New Mpin and Confirm Mpin didn''t match' message, null id
			return
		end
		else
		begin
			update tbl_user_detail set m_pin = PWDENCRYPT(@user_new_mPin) where user_email = @user_name or user_mobile_no = @user_name or user_id = @user_id
			select '0' code, 'User MPin updated succesfully' message , null id
			return
		end
	end

	--resend verification code
	if @flag = 'rv'
	begin
		--select * from tbl_User_registration
		
		--in user signup resend code case,app will send user email, mobile and full name again

		if @new_user_mobileNumber is not null and @new_user_email is not null and @new_user_full_name is not null
		begin
		-- first delete the existing user entry
			delete from tbl_User_registration where Agent_Mobile_Number = @new_user_mobileNumber and Agent_Email_address = @new_user_email

			--send registration again
			exec sproc_Agent_Registration @flag = 'i', @agent_mobile_number = @new_user_mobileNumber, @agent_email_address = @new_user_email,@agent_full_name = @new_user_full_name
		end
		-- in any other cases, app will have username in session and will send the username

		if @user_name is not null
		begin
			select @new_user_email = user_email, @new_user_mobileNumber = user_mobile_no, @new_user_full_name = full_name from tbl_user_detail u where user_name = @user_name

			--set @gen_verification_code = dbo.[func_generate_random_number] ((@new_user_mobileNumber) + cast(GEtDate() as varchar))

			insert into tbl_User_registration(Agent_Email_address, Agent_full_Name, Agent_Mobile_Number, Agent_verification_code,  generate_date_time,username)
			values(@new_user_email, @new_user_full_name, @new_user_mobileNumber, @gen_verification_code, GETDATE(), @user_name)

			exec sproc_email_request @flag = 'd',@full_name = @new_user_full_name, @agent_verification_code =@gen_verification_code, @email_id = @new_user_email   
			update tbl_User_registration set Agent_verification_Status = 'Sent',send_date_time=getdate() where username = @user_name
		end
	end

		--User sign Up
	if @flag = 'sp'
	begin
		if exists(select 'x' from tbl_user_detail where user_mobile_no = @new_user_mobileNumber or user_email = @new_user_email)
		begin
			select '159' code, 'User with the provided details already exists, please use forgot password ' message, null id
			return
		end
		else if exists (select 'x' from tbl_User_registration where Agent_Email_address = @new_user_email and Agent_Mobile_Number = @new_user_mobileNumber and Isnull(Agent_verification_Status,'Sent') = trim('Verified'))
		begin
			Select '1' Code, 'User already Verified'Message , null id
			return
		end
		else
		begin
			--send email verification code by email/sms

			insert into tbl_User_registration(Agent_Mobile_Number, Agent_Email_address,Agent_full_Name,Agent_verification_code,generate_date_time) values(@new_user_mobileNumber, @new_user_email, @new_user_full_name, @gen_verification_code,getdate())
			exec sproc_email_request @flag = 'd',@full_name = @new_user_full_name, @agent_verification_code =@gen_verification_code, @email_id = @new_user_email 
			update tbl_User_registration set Agent_verification_Status = 'Sent',send_date_time=getdate() where Agent_Mobile_Number = @new_user_mobileNumber and Agent_Email_address =@new_user_email and Agent_verification_code=@gen_verification_code

	
			select '0' code, 'Verification code sent succesfully' message, null id
			return
		end
		

	end

END




