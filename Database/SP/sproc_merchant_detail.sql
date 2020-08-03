USE [WePayNepal]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sproc_merchant_detail] @flag char(8) = NULL,
@action_user varchar(50) = NULL,
@user_id int = NULL,
@agent_id int = NULL,
@parent_id int = NULL,
@merchant_type varchar(20) = NULL,
@merchant_operation_type varchar(15) = NULL,
@merchant_commission_type bit = NULL,
@merchant_name varchar(512) = NULL,
@merchant_phone_number varchar(15) = NULL,
@merchant_mobile_no varchar(10) = NULL,
@merchant_email varchar(512) = NULL,
@merchant_web_url varchar(512) = NULL,
@merchant_registration_no varchar(512) = NULL,
@merchant_Pan_no varchar(512) = NULL,
@merchant_contract_date datetime = NULL,
@merchant_province varchar(512) = NULL,
@merchant_district varchar(512) = NULL,
@merchant_local_body varchar(512) = NULL,
@merchant_ward_number int = NULL,
@merchant_street varchar(512) = NULL,
@merchant_country varchar(512) = NULL,
@merchant_credit_limit decimal(18, 2) = NULL,
@merchant_available_balance decimal(18, 2) = NULL,
@merchant_logo varchar(max) = NULL,
@merchant_reg_certificate varchar(max) = NULL,
@merchant_pan_Certificate varchar(max) = NULL,
@merchant_commission_cat_id int = NULL,
@merchant_nationality varchar(512) = NULL,
@action_ip varchar(20) = NULL,
@action_platform varchar(30) = NULL,
-----user details-------  
@user_name varchar(50) = NULL,
@password varchar(512) = NULL,
@confirm_password varchar(512) = NULL,
@first_name varchar(512) = NULL,
@middle_name varchar(512) = NULL,
@last_name varchar(512) = NULL,
@user_mobile_number varchar(10) = NULL,
@user_email varchar(512) = NULL,
----contact person for business agent  
@contact_person_name varchar(512) = NULL,
@contact_person_mobile_number varchar(10) = NULL,
@contact_person_ID_type varchar(512) = NULL,
@contact_person_ID_no varchar(512) = NULL,
@contact_person_Id_issue_date varchar(512) = NULL,
@contact_person_id_issue_date_nepali varchar(512) = NULL,
@contact_person_id_expiry_date varchar(512) = NULL,
@contact_person_id_expiry_date_nepali varchar(512) = NULL,
@contact_person_id_issue_district varchar(512) = NULL,
@contact_person_address varchar(512) = NULL,
@role_id int = NULL,
@usr_type_id int = NULL,
@action_browser varchar(512) = NULL,
@is_primary char(3) = NULL,
@agent_status varchar(51) = NULL,
@end_date datetime = NULL,
@from_date datetime = NULL,
@user_status varchar(20) = NULL,
@usr_type varchar(20) = NULL

AS
  DECLARE @sql nvarchar(max),
          @currentdate datetime,
          @last_online datetime,
          @merchant_code varchar(512),
          @merchant_country_code char(3),
          @id int

  BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    IF @flag = 's' -- merchant user list                
    BEGIN
      SET @currentdate = dbo.func_get_nepali_date(DEFAULT);
      SET @sql = 'SELECT
     [agent_id]
      ,[parent_id]
      ,[agent_code]
      ,[agent_type]
      ,[agent_operation_type]
      ,[agent_name]
      ,[kyc_status]
      ,[first_name]
      ,[middle_name]
      ,[last_name]
      ,[available_balance]
      ,[date_of_birth_eng]
      ,[date_of_birth_nep]
      ,[gender]
      ,[agent_phone_no]
      ,[agent_mobile_no]
      ,[agent_email_address]
      ,[occupation]
      ,[marital_status]
      ,[spouse_name]
      ,[father_name]
      ,[mother_name]
      ,[grand_father_name]
      ,[agent_nationality]
      ,[agent_country]
      ,[permanent_province]
      ,[permanent_district]
      ,[permanent_localbody]
      ,[permanent_wardno]
      ,[permanent_address]
      ,[temporary_province]
      ,[temporary_district]
      ,[temporary_localbody]
      ,[temporary_wardno]
      ,[temporary_address]
      ,[latitude]
      ,[longitude]
      ,[web_url]
      ,[agent_registration_no]
      ,[agent_pan_no]
      ,[agent_credit_limit]
      ,[agent_support_staff]
      ,format(agent_contract_local_date,''yyyy-MM-dd'')agent_contract_local_date
      ,[agent_contract_nepali_date]
      ,[agent_logo_img]
      ,[agent_document_img_front]
      ,[agent_document_img_back]
      ,[contact_person_name]
      ,[agent_country_code]
      ,[contact_person_mobile_no]
      ,[contact_person_id_type]
      ,[contact_person_id_no]
      ,format(contact_id_issue_local_date,''yyyy-MM-dd'')contact_id_issue_local_date
      ,[contact_id_issued_bs_date]
      ,[contact_id_issued_district]
      ,[agent_commission_id]
      ,[agent_status]
      ,[is_auto_commission]
      ,[agent_qr_image]
      ,[fund_load_reward]
      ,[txn_reward_point]
      ,[admin_remarks]
      ,[full_name]
      ,[is_sameAs_per_add]
      ,[individual_image]
      ,[referal_id]
      ,[agent_referal_id]
      ,[lock_status]
      ,[locked_reason]
      ,[locked_UTC_date]
      ,[locked_by]
      ,[created_UTC_date]
      ,[created_local_date]
      ,[created_nepali_date]
      ,[created_by]
      ,[created_ip]
      ,[created_platform]
      ,[updated_by]
      ,[updated_UTC_date]
      ,[updated_local_date]
      ,[updated_nepali_date]
      ,[updated_ip]
      ,[agent_address]
      ,[contact_Person_address]
    FROM tbl_agent_detail WHERE 1=1  '

      IF (@merchant_type IS NOT NULL)
      BEGIN
        SET @sql = @sql + ' AND agent_type = ''' + @merchant_type + '''';
      END;

      IF (@agent_id IS NOT NULL)
      BEGIN
        SET @sql = @sql + ' AND agent_id = ''' + @merchant_type + '''';
      END;

      IF @parent_id IS NOT NULL
      BEGIN
        SET @sql = @sql + ' AND parent_id = ''' + CAST(@parent_id AS varchar) + '''';
      END;

      PRINT @sql;

      EXEC (@sql);
    END;

    IF @flag = 'i' -- insert new merchant  
    BEGIN
      IF EXISTS (SELECT
          'x'
        FROM tbl_agent_detail
        WHERE agent_email_address = @merchant_email)
      BEGIN
        SELECT
          '1' Code,
          'Email Address already exists' Message,
          NULL id;

        RETURN;
      END;

	  IF EXISTS (SELECT
          'x'
        FROM tbl_user_detail
        WHERE user_name = @user_name)
      BEGIN
        SELECT
          '1' Code,
          'User Name already exists' Message,
          NULL id;

        RETURN;
      END;

      IF EXISTS (SELECT
          'x'
        FROM tbl_agent_detail
        WHERE agent_mobile_no = @merchant_mobile_no)
      BEGIN
        SELECT
          '1' Code,
          'Merchant Mobile Number already exists' Message,
          NULL id;

        RETURN;
      END;
	  
	  IF EXISTS (SELECT
          'x'
        FROM tbl_user_detail
        WHERE user_mobile_no = @merchant_mobile_no)
      BEGIN
        SELECT
          '1' Code,
          'User Mobile Number already exists' Message,
          NULL id;

        RETURN;
      END;


      SELECT
        @agent_id = MAX(agent_id)
      FROM tbl_agent_detail;

      SELECT
        @merchant_code = RIGHT('0000000000' + CAST((ISNULL(@agent_id, 0) + 1) AS varchar(10)), 10);

      SET @merchant_country = ISNULL(@merchant_country, 'Nepal');
      SET @merchant_country_code = ISNULL(@merchant_country_code, 'NPL');

      IF @merchant_commission_cat_id IS NULL
      BEGIN
        SELECT
          @merchant_commission_cat_id = category_id
        FROM tbl_commission_category
        WHERE category_name = 'Default';
      END;
	  --Print ('x')
	  --return
      INSERT INTO [dbo].[tbl_agent_detail] ([parent_id]
      , [agent_code]
      , [agent_type]
      , [agent_operation_type]
      , [agent_name]
      , first_name
      , middle_name
      , last_name
      , full_name
      , [available_balance]
      , [agent_phone_no]
      , [agent_mobile_no]
      , [agent_email_address]
      , [agent_nationality]
      , [agent_country]
      , [agent_country_code]
      , permanent_province
      , permanent_district
      , [permanent_localbody]
      , [permanent_wardno]
      , [permanent_address]
      , [web_url]
      , [agent_registration_no]
      , [agent_pan_no]
      , [agent_credit_limit]
      , [agent_contract_local_date]
      , [agent_contract_nepali_date]
      , [agent_logo_img]
      , [agent_document_img_front]
      , [agent_document_img_back]
      , [contact_person_name]
      , [contact_person_mobile_no]
      , [contact_person_id_type]
      , [contact_person_id_no]
      , [contact_id_issue_local_date]
      , [contact_id_issued_bs_date]
      , [contact_id_issued_district]
      , contact_person_address
      , [agent_commission_id]
      , [agent_status]
      , [is_auto_commission]
      , [fund_load_reward]
      , [txn_reward_point]
      , [lock_status]
      , [created_UTC_date]
      , [created_local_date]
      , [created_nepali_date]
      , [created_by]
      , [created_ip]
      , [created_platform]
      , kyc_status)
        VALUES (@parent_id, @merchant_code, @merchant_type, @merchant_operation_type, @merchant_name, @first_name, 
		@middle_name, @last_name, ISNULL(@first_name, '') + ' ' + ISNULL(@middle_name, '') + ' ' + ISNULL(@last_name, ''), 
		@merchant_available_balance, @merchant_phone_number, @merchant_mobile_no, @merchant_email, ISNULL(@merchant_nationality, 'Nepali'), @merchant_country, @merchant_country_code, @merchant_province, @merchant_district, @merchant_local_body, @merchant_ward_number, @merchant_street, @merchant_web_url, @merchant_registration_no, @merchant_Pan_no, @merchant_credit_limit, @merchant_contract_date, dbo.func_get_nepali_date(@merchant_contract_date), @merchant_logo, @merchant_pan_Certificate, @merchant_reg_certificate, @contact_person_name, @contact_person_mobile_number, @contact_person_ID_type, @contact_person_ID_no, @contact_person_Id_issue_date, @contact_person_id_expiry_date_nepali, @contact_person_id_issue_district, @contact_person_address, @merchant_commission_cat_id, 'y', @merchant_commission_type, 0, 0, 'n', GETUTCDATE(), GETDATE(), dbo.func_get_nepali_date(DEFAULT), @action_user, @action_ip, @action_platform, 'Pending')

      SET @id = SCOPE_IDENTITY()

      INSERT INTO [dbo].[tbl_user_detail] ([agent_id]
      , [role_id]
      , [usr_type_id]
      , [usr_type]
      , [user_name]
      , [full_name]
      , [password]
      , [user_email]
      , [user_mobile_no]
      , [status]
      , [created_UTC_date]
      , [created_local_date]
      , [created_nepali_date]
      , [created_by]
      , [created_ip]
      , [created_platform]
      , [allow_multiple_login]
      , [is_login_enabled]
      , [is_primary]
      , [browser_info])
        VALUES (@id, @role_id, @usr_type_id, @usr_type, @user_name, ISNULL(@first_name, '') + ' ' + ISNULL(@middle_name, '') + ' ' + ISNULL(@last_name, ''), 
		PWDENCRYPT(@password), @user_email, @user_mobile_number, 'y', GETUTCDATE(), GETDATE(), dbo.func_get_nepali_date(DEFAULT), @action_user, @action_ip, @action_platform, 'y', 'y', 'y', @action_browser)
      SELECT
        '0' STATUS,
        'Merchant created successfully' Message,
        NULL id;
      RETURN
    END;


    IF @flag = 'u'  --update agent/user detail  
    BEGIN
      IF (@agent_id IS NULL)
      BEGIN
        EXEC sproc_error_handler @error_code = '1',
                                 @Msg = 'MERCHANT ID CANNOT BE NULL',
                                 @id = NULL;

        RETURN;
      END;

      IF ((SELECT
          COUNT(*)
        FROM tbl_agent_detail
        WHERE agent_id = @agent_id)
        = 0
        )
      BEGIN
        EXEC sproc_error_handler @error_code = '1',
                                 @msg = 'Merchant not found',
                                 @id = @agent_id;

        RETURN;
      END;

      IF @user_id IS NOT NULL
      BEGIN
        IF NOT EXISTS (SELECT
            'x'
          FROM tbl_user_detail
          WHERE user_id = @user_id)
        BEGIN
          SELECT
            '1' code,
            'User not Found' message,
            NULL id

          RETURN
        END
      END

      UPDATE [dbo].[tbl_agent_detail]
      SET [agent_operation_type] = ISNULL(@merchant_operation_type, agent_operation_type),
          [agent_name] = ISNULL(@merchant_name, agent_name),
          [agent_phone_no] = ISNULL(@merchant_phone_number, agent_phone_no),
          [agent_mobile_no] = ISNULL(@merchant_mobile_no, agent_mobile_no),
          [agent_email_address] = ISNULL(@merchant_email, agent_email_address),
          [first_name] = ISNULL(@first_name, first_name),
          [middle_name] = ISNULL(@middle_name, middle_name),
          [last_name] = ISNULL(@last_name, last_name),
          [agent_nationality] = ISNULL(@merchant_nationality, agent_nationality),
          [agent_country] = ISNULL(@merchant_country, agent_country),
          [agent_country_code] = ISNULL(@merchant_country_code, agent_country_code),
          [permanent_province] = ISNULL(@merchant_province, [permanent_province]),
          [permanent_district] = ISNULL(@merchant_district, [permanent_district]),
          [permanent_localbody] = ISNULL(@merchant_local_body, [permanent_localbody]),
          [permanent_wardno] = ISNULL(@merchant_ward_number, [permanent_wardno]),
          [agent_address] = ISNULL(@merchant_street, agent_address),
          [web_url] = ISNULL(@merchant_web_url, web_url),
          [agent_registration_no] = ISNULL(@merchant_registration_no, agent_registration_no),
          [agent_pan_no] = ISNULL(@merchant_Pan_no, agent_pan_no),
          [agent_credit_limit] = ISNULL(@merchant_credit_limit, agent_credit_limit),
          [agent_contract_local_date] = ISNULL(@merchant_contract_date, agent_contract_local_date),
          [agent_contract_nepali_date] = ISNULL(dbo.func_get_nepali_date(@merchant_contract_date), agent_contract_nepali_date),
          [agent_logo_img] = ISNULL(@merchant_logo, agent_logo_img),
          [agent_document_img_front] = ISNULL(@merchant_pan_Certificate, [agent_document_img_front]),
          [agent_document_img_back] = ISNULL(@merchant_reg_certificate, [agent_document_img_back]),
          [contact_person_name] = ISNULL(@contact_person_name, contact_person_name),
          [contact_person_mobile_no] = ISNULL(@contact_person_mobile_number, contact_person_mobile_no),
          [contact_person_id_type] = ISNULL(@contact_person_ID_type, contact_person_id_type),
          [contact_person_id_no] = ISNULL(@contact_person_ID_no, contact_person_id_no),
          [contact_id_issue_local_date] = ISNULL(@contact_person_Id_issue_date, contact_id_issue_local_date),
          [contact_id_issued_bs_date] = ISNULL(@contact_person_id_issue_date_nepali, contact_id_issued_bs_date),
          [contact_id_issued_district] = ISNULL(@contact_person_id_issue_district, contact_id_issued_district),
          contact_person_address = ISNULL(@contact_person_address, contact_person_address),
          [agent_commission_id] = ISNULL(@merchant_commission_cat_id, agent_commission_id),
          [is_auto_commission] = ISNULL(@merchant_commission_type, is_auto_commission),
          [updated_by] = ISNULL(@action_user, updated_by),
          [updated_UTC_date] = ISNULL(GETUTCDATE(), updated_UTC_date),
          [updated_local_date] = ISNULL(GETDATE(), updated_local_date),
          [updated_nepali_date] = ISNULL(dbo.func_get_nepali_date(DEFAULT), updated_nepali_date),
          [updated_ip] = ISNULL(@action_ip, updated_ip)
      WHERE agent_id = @agent_id

      UPDATE [dbo].[tbl_user_detail]
      SET [role_id] = ISNULL(@role_id, role_id),
          [usr_type_id] = ISNULL(@usr_type_id, usr_type_id),
          [usr_type] = ISNULL(@usr_type, usr_type),
          [user_name] = ISNULL(@user_name, user_name),
          [full_name] = ISNULL((ISNULL(@first_name, '') + '' + ISNULL(@middle_name, '') + '' + ISNULL(@last_name, '')), full_name),
          [password] = ISNULL(PWDENCRYPT(@password), password),
          [user_email] = ISNULL(@user_email, user_email),
          [user_mobile_no] = ISNULL(@user_mobile_number, user_mobile_no),
          [updated_by] = ISNULL(@action_user, updated_by),
          [updated_UTC_date] = ISNULL(GETUTCDATE(), updated_UTC_date),
          [updated_local_date] = ISNULL(GETDATE(), updated_local_date),
          [updated_nepali_date] = ISNULL(dbo.func_get_nepali_date(DEFAULT), updated_nepali_date),
          [updated_ip] = ISNULL(@action_ip, updated_ip),
          [is_primary] = ISNULL(@is_primary, is_primary),
          [browser_info] = ISNULL(@action_browser, browser_info)
      WHERE user_id = @user_id

      SELECT
        '0' code,
        'Agent updated succesfully' message,
        NULL id
      RETURN

    END

    IF @flag = 'md'  --select merchant detail  
    BEGIN
      IF NOT EXISTS (SELECT
          'x'
        FROM tbl_agent_detail
        WHERE agent_id = @agent_id)
      BEGIN
        SELECT
          '1' code,
          'Merchant not found ' message,
          NULL id
        RETURN

      END
      ELSE
      BEGIN
        SELECT
          *
        FROM tbl_agent_detail ad
        JOIN tbl_user_detail u
          ON u.agent_id = ad.agent_id
        WHERE ad.agent_id = @agent_id
      END
    END

    IF @flag = 'edm'   --Enable/Disable Merchant Status
    BEGIN
      IF NOT EXISTS (SELECT
          'x'
        FROM tbl_user_detail
        WHERE user_id = @user_id)
      BEGIN
        SELECT
          '1' code,
          'Merchant Not Found' message,
          NULL id

        RETURN
      END

      IF NOT EXISTS (SELECT
          'x'
        FROM tbl_agent_detail
        WHERE agent_id = @agent_id)
      BEGIN
        SELECT
          '1' code,
          'Merchant not found' message,
          NULL id

        RETURN
      END

      UPDATE tbl_agent_detail
      SET agent_status = @user_status
      WHERE agent_id = @agent_id

      UPDATE tbl_user_detail
      SET STATUS = @user_status
      WHERE user_id = @user_id

      SELECT
        '0' code,
        'User Status updated succesfully' message,
        NULL id

      RETURN
    END

  END
GO