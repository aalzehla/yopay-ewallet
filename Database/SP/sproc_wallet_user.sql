USE [WePayNepal]
GO
/****** Object:  StoredProcedure [dbo].[sproc_wallet_user]    Script Date: 6/7/2020 1:53:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
ALTER PROCEDURE [dbo].[sproc_wallet_user] @flag CHAR(3)
	,@user_name VARCHAR(512) = NULL
	,@user_id INT = NULL
	,@agent_id INT = NULL
	,@parent_id INT = NULL
	,@agent_type VARCHAR(512) = NULL
	,@fullname VARCHAR(255) = NULL
	,@usermobileNumber VARCHAR(10) = NULL
	,@useremail VARCHAR(255) = NULL
	,@action_user VARCHAR(512) = NULL
	,@action_IP VARCHAR(50) = NULL
	,@action_browser VARCHAR(255) = NULL
	,@status VARCHAR(512) = NULL
AS
DECLARE @sql VARCHAR(max),@randmon_pasword varchar(100)

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from  
	-- interfering with SELECT statements.  
	SET NOCOUNT ON;

	IF @flag = 's' -- wallet user list                
	BEGIN
		SET @sql = 'SELECT u.user_id, u.user_mobile_no, u.user_email, u.full_name, u.agent_id, a.parent_id, a.kyc_status, u.status as userStatus,CONVERT(VARCHAR(10),  u.created_local_date, 103) as Registered_Date,isnull(a.available_balance, 0 ) as balance FROM
 tbl_user_detail  u  
join tbl_agent_detail a   
on a.agent_id  = u.agent_id  
WHERE 1=1 ';

		--  if @agent_id is not null  
		--  begin  
		--SET @sql = @sql + ' AND Agent_Id = ''' + CAST(@agent_id AS VARCHAR) + '''';    
		--  end  
		IF (@agent_type IS NOT NULL)
		BEGIN
			SET @sql = @sql + ' AND a.Agent_Type = ''' + @agent_type + '''';
		END;

		IF @parent_id IS NOT NULL
		BEGIN
			SET @sql = @sql + ' AND parent_id = ''' + CAST(@parent_id AS VARCHAR) + '''';
		END;

		PRINT @sql;

		EXEC (@sql);
	END;

	--enable/disable agent  
	IF @flag = 'b'
	BEGIN
		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_user_detail
				WHERE user_id = @user_id
				)
		BEGIN
			SELECT '1' code
				,'User Not Found' message
				,NULL id

			RETURN
		END

		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_agent_detail
				WHERE agent_id = @agent_id
				)
		BEGIN
			SELECT '1' code
				,'agent not found' message
				,NULL id

			RETURN
		END

		UPDATE tbl_agent_detail
		SET agent_status = @status
		WHERE agent_id = @agent_id

		UPDATE tbl_user_detail
		SET STATUS = @status
		WHERE user_id = @user_id

		SELECT '0' code
			,'User Status updated succesfully' message
			,NULL id

		RETURN
	END

	IF @flag = 'i'
	BEGIN
	IF EXISTS (SELECT
          'x'
        FROM tbl_user_detail
        WHERE user_email = @useremail
		OR user_mobile_no= @usermobileNumber
		)
      BEGIN
        SELECT
          '1' Code,
          'User Alredy Exist' Message,
          NULL id;

        RETURN;
      END;
		IF NOT EXISTS (
				SELECT 'x'
				FROM tbl_agent_detail
				WHERE agent_email_address = @useremail
					OR agent_mobile_no = @usermobileNumber
				)
		BEGIN
		set @randmon_pasword = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9)
			INSERT INTO tbl_agent_detail (
				agent_mobile_no
				,agent_email_address
				,agent_name
				,full_name
				,agent_type
				,agent_operation_type
				,kyc_status
				,agent_status
				,is_auto_commission
				,parent_id
				,created_UTC_date
				,created_local_date
				,created_nepali_date
				,created_by
				,created_ip
				)
			VALUES (
				@usermobileNumber
				,@useremail
				,@fullname
				,@fullname
				,'WalletUser'
				,'I'
				,'N'
				,'Y'
				,0
				,@parent_id
				,GETUTCDATE()
				,GETDATE()
				,dbo.func_get_nepali_date(GETDATE())
				,@action_user
				,@action_IP
				);

			SET @agent_id = SCOPE_IDENTITY();

			INSERT INTO tbl_user_detail (
				user_name
				,password
				,full_name
				,agent_id
				,user_email
				,user_mobile_no
				,created_UTC_date
				,created_local_date
				,created_nepali_date
				,created_by
				,created_ip
				,STATUS
				,usr_type
				,usr_type_id
				,role_id
				,is_primary
				)
			VALUES (
				'User' + @usermobileNumber
				,PWDENCRYPT(@randmon_pasword)
				,@fullname
				,@agent_id
				,@useremail
				,@usermobileNumber
				,GETUTCDATE()
				,GETDATE()
				,dbo.func_get_nepali_date(GETDATE())
				,@action_user
				,@action_IP
				,'Y'
				,'WalletUser'
				,6
				,6
				,'Y'
				);

			exec sproc_email_request @flag = 'dr',@full_name = @fullname, @email_id = @useremail ,@password = @randmon_pasword 

			SELECT '0' code
				,'User Created succesfully' message
		END
		ELSE
		BEGIN
			SELECT '1' code
				,'User Alredy Exist' message
		END
	END
END
