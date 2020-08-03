USE [WePayNepal]
GO

/****** Object:  StoredProcedure [dbo].[sproc_txn_request]    Script Date: 6/4/2020 5:36:40 PM ******/
DROP PROCEDURE [dbo].[sproc_txn_request]
GO

/****** Object:  StoredProcedure [dbo].[sproc_txn_request]    Script Date: 6/4/2020 5:36:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE     PROCEDURE [dbo].[sproc_txn_request] @flag             CHAR(5)        = NULL, 
                                                    @product_id       INT            = NULL, 
                                                    @subscriber_no    VARCHAR(100)   = NULL, 
                                                    @amount           DECIMAL(18, 2)  = NULL, 
                                                    @agent_id         INT            = NULL, 
                                                    @status           VARCHAR(20)    = NULL, 
                                                    @created_ip       VARCHAR(10)    = NULL, 
                                                    @created_platform VARCHAR(10)    = NULL, 
                                                    @updated_by       VARCHAR(20)    = NULL, 
                                                    @process_id       VARCHAR(100)   = NULL, 
                                                    @service_charge   DECIMAL(18, 2)  = NULL, 
                                                    @txn_id           VARCHAR(30)    = NULL, 
                                                    @additional_data  NVARCHAR(MAX)  = NULL, 
                                                    @action_user      VARCHAR(100)   = NULL, 
                                                    @extra_field1     NVARCHAR(MAX)  = NULL, 
                                                    @extra_field2     VARCHAR(100)   = NULL, 
                                                    @extra_field3     VARCHAR(100)   = NULL, 
                                                    @extra_field4     VARCHAR(100)   = NULL, 
                                                    @remarks          VARCHAR(1000)  = NULL, 
                                                    @session_id       VARCHAR(200)   = NULL, 
                                                    @customer_id      VARCHAR(100)   = NULL, 
                                                    @customer_name    VARCHAR(100)   = NULL, 
                                                    @plan_id          VARCHAR(100)   = NULL, 
                                                    @plan_name        VARCHAR(100)   = NULL, 
                                                    @quantity         INT            = NULL, 
                                                    @policy_number    VARCHAR(20)    = NULL, 
                                                    @document_number  VARCHAR(20)    = NULL, 
                                                    @name             VARCHAR(MAX)   = NULL, 
                                                    @date_of_birth    VARCHAR(20)    = NULL, 
                                                    @bill_amount      VARCHAR(20)    = NULL, 
                                                    @extra_field5     VARCHAR(20)    = NULL, 
                                                    @extra_field6     VARCHAR(20)    = NULL, 
                                                    @extra_field7     VARCHAR(20)    = NULL, 
                                                    @transaction_type VARCHAR(20)    = NULL, 
                                                    @location_id      VARCHAR(20)    = NULL, 
                                                    @location_name    VARCHAR(500)   = NULL, 
                                                    @branch_id        VARCHAR(20)    = NULL, 
                                                    @branch_name      VARCHAR(500)   = NULL, 
                                                    @receiver_name    VARCHAR(100)   = NULL, 
                                                    @account_number   VARCHAR(100)   = NULL, 
                                                    @receiver_contact VARCHAR(100)   = NULL, 
                                                    @receiver_city    VARCHAR(100)   = NULL, 
                                                    @receiver_address VARCHAR(100)   = NULL, 
                                                    @sender_name      VARCHAR(100)   = NULL, 
                                                    @sender_contact   VARCHAR(100)   = NULL, 
                                                    @sender_city      VARCHAR(100)   = NULL, 
                                                    @sender_address   VARCHAR(100)   = NULL, 
                                                    @sender_id_type   VARCHAR(100)   = NULL, 
                                                    @sender_id_number VARCHAR(100)   = NULL, 
                                                    @user_type        VARCHAR(100)   = NULL, 
                                                    @kyc_status       VARCHAR(20)    = NULL, 
                                                    @card_no          VARCHAR(16)    = NULL,
													@card_amount decimal(18,2) =  null
AS
    BEGIN
        DECLARE @txn_type INT, @company_id INT, @agent_type VARCHAR(100), @gateway_id INT, @is_auto_commission CHAR(5), @agent_status CHAR(100), @commission_id INT, @agent_lock_status CHAR(3), @agent_credit_limit FLOAT;
        DECLARE @parent_id INT, @parent_status VARCHAR(30), @parent_auto_commission CHAR(3), @parent_commission_id INT, @parent_lock_status CHAR(3), @parent_credit_limit FLOAT;
        DECLARE @gparent_id INT, @grand_parent_status VARCHAR(30), @gparent_auto_commission CHAR(3), @gparent_commission_id INT, @gparent_lock_status CHAR(3), @gparent_credit_limit FLOAT, @gateway_balance DECIMAL(18, 2);
        DECLARE @pgateway_id INT, @sgateway_id INT, @product_status VARCHAR(20), @product VARCHAR(130), @bonus_amt MONEY, @extra_field NVARCHAR(MAX), @gateway_bill_number VARCHAR(20), @commission_amount FLOAT;
        DECLARE @admin_comm_rate FLOAT, @admin_rate_type CHAR(3), @cost_amount_admin DECIMAL(18, 2), @admin_commission_earned DECIMAL(18, 2), @gdist_comm_rate FLOAT, @gdist_rate_type CHAR(3);
        DECLARE @dist_comm_rate FLOAT, @dist_rate_type CHAR(3), @dist_cost_amount DECIMAL(18, 2), @category_id INT, @merchant_rate FLOAT, @merchant_cost_amount DECIMAL(18, 2), @last_client_balance DECIMAL(18, 2);
        DECLARE @cost_amount DECIMAL(18, 2), @txn_date DATETIME, @agent_balance DECIMAL(18, 2), @record_amount DECIMAL(18, 2), @pdist_comm_rate FLOAT, @pdist_rate_type CHAR(3), @txn_amount decimal(18,2)
        DECLARE @dist_comm_earned DECIMAL(18, 2), @gdist_comm_earned DECIMAL(18, 2), @pdist_comm_earned DECIMAL(18, 2), @merchant_comm_earned DECIMAL(18, 2), @pdist_cost_amount DECIMAL(18, 2), @gdist_cost_amount DECIMAL(18, 2), @user_id INT;
        DECLARE @sql VARCHAR(MAX), @transaction_limit_max DECIMAL(18, 2), @daily_max_limit DECIMAL(18, 2), @monthly_max_limit DECIMAL(18, 2), @daily_txn_amount DECIMAL(18, 2), @monthly_txn_amount DECIMAL(18, 2), @card_type_label VARCHAR(50), @card_type INT
        SET NOCOUNT ON;
        IF @gateway_id IS NULL
            SELECT @gateway_id = ISNULL(primary_gateway, secondary_gateway)
            FROM tbl_manage_services
            WHERE product_id = @product_id;
        BEGIN TRY
            IF @flag = 'chk' -- check if the same txnid is already a success/processing case and do not allow retry. for other cases, retry is allowed.                                                                                                
                BEGIN
                    SELECT @agent_id = agent_id, 
                           @record_amount = amount, 
                           @subscriber_no = subscriber_no, 
                           @txn_date = created_local_date
                    FROM tbl_transaction
                    WHERE txn_id = @txn_id;
                    SELECT @agent_balance = available_balance
                    FROM tbl_agent_detail
                    WHERE agent_id = @agent_id;
                    IF @amount <= 0
                        BEGIN
                            SELECT '1' code, 
                                   'Invalid amount, should be greater than 0' message, 
                                   NULL id;
                            RETURN;
                    END;-- amount less than 0             

                    IF @amount > @agent_balance
                        BEGIN
                            SELECT '1' code, 
                                   'Your balance is less than topup amount' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF @product_id IS NULL -- product id null                            
                        BEGIN
                            SELECT '1' code, 
                                   'product id is blank' message, 
                                   NULL id;
                            RETURN;
                    END;
                    PRINT(@amount);
                    PRINT(@record_amount);
                    IF(@record_amount <> @amount
                       OR @subscriber_no <> @subscriber_no)
                        BEGIN
                            SELECT '1' code, 
                                   'illegal attempt' message, 
                                   @cost_amount sellingprice, 
                                   @txn_date txn_date, 
                                   NULL txn_date2;
                            RETURN;
                    END;
                    SELECT '0' code, 
                           'check was successful' message, 
                           format(@txn_date, 'dd.mm.yyyy hh:mm:ss') txn_date, 
                           @cost_amount sellingprice, 
                           format(@txn_date, 'yyyy-mm-ddthh:mm:ss') txn_date2;
                    RETURN;
            END;
            IF @flag = 'i'
                BEGIN
                    IF EXISTS
                    (
                        SELECT 'x'
                        FROM tbl_transaction
                        WHERE txn_id = @txn_id
                    )
                        BEGIN
                            SELECT '1' code, 
                                   'Transaction already exists' message, 
                                   NULL id;
                            RETURN;
                    END;-- duplicate txn check                            

                    IF @amount <= 0
                        BEGIN
                            SELECT '1' code, 
                                   'Invalid amount, Amount should be greater than 0' message, 
                                   NULL id;
                            RETURN;
                    END;-- amount less than 0                            

                    IF @product_id IS NULL -- product id null                            
                        BEGIN
                            SELECT '1' code, 
                                   'Product id is blank' message, 
                                   NULL id;
                            RETURN;
                    END;
                    SELECT @agent_id = agent_id, 
                           @user_id = [user_id]
                    FROM tbl_user_detail
                    WHERE [user_name] = @action_user
                          OR user_mobile_no = @action_user
                          OR user_email = @action_user;-- get agent_id from user_id                          

                    IF @agent_id IS NULL
                        BEGIN
                            SELECT '1' code, 
                                   'Cannot find agent id' message, 
                                   NULL id;
                            RETURN;
                    END;-- agent id null/invalid                             

                    SELECT @agent_status = agent_status, 
                           @agent_balance = ISNULL(available_balance, 0), 
                           @agent_type = agent_type, 
                           @is_auto_commission = ISNULL(is_auto_commission, 1), 
                           @agent_lock_status = lock_status, 
                           @commission_id = agent_commission_id, 
                           @agent_credit_limit = ISNULL(agent_credit_limit, 0), 
                           @parent_id = parent_id, 
                           @kyc_status = CASE kyc_status
                                             WHEN 'Approved'
                                             THEN 'verified'
                                             ELSE 'not verified'
                                         END
                    FROM tbl_agent_detail
                    WHERE agent_id = @agent_id;
                    SELECT @transaction_limit_max = transaction_limit_max, 
                           @daily_max_limit = transaction_daily_limit_max, 
                           @monthly_max_limit = transaction_monthly_limit_max
                    FROM tbl_NRB_transaction_limit
                    WHERE KYC_Status = @kyc_status
                          AND txn_type = 'Wallet Payment'; --Wallet Payment  

                    SELECT @daily_txn_amount = ISNULL(SUM(CASE
                                                              WHEN(DAY(created_UTC_date) = DAY(GETUTCDATE())
                                                                   AND MONTH(created_UTC_date) = MONTH(GETUTCDATE())
                                                                   AND YEAR(created_UTC_date) = YEAR(GETUTCDATE()))
                                                              THEN amount
                                                          END), 0), 
                           @monthly_txn_amount = ISNULL(SUM(CASE
                                                                WHEN(MONTH(created_UTC_date) = MONTH(GETUTCDATE())
                                                                     AND YEAR(created_UTC_date) = YEAR(GETUTCDATE()))
                                                                THEN amount
                                                            END), 0)
                    FROM tbl_transaction
                    WHERE STATUS = 'success'
                          AND agent_id = @agent_id;
                    IF @amount > @transaction_limit_max
                        BEGIN
                            SELECT '1' code, 
                                   'Transaction Limit Exceeded' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF(@amount + @monthly_txn_amount) > @monthly_max_limit
                        BEGIN
                            SELECT '1' code, 
                                   'Monthly Transaction Limit Exceeded' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF(@amount + @daily_txn_amount) > @daily_max_limit
                        BEGIN
                            SELECT '1' code, 
                                   'Daily Transaction Limit Exceeded' message, 
                                   NULL id;
                            RETURN;
                    END;

                    --select @agentstatus agentstatus, @agenttype agenttype,@is_auto_commission autocommsission,  @agentlockstatus lockstatus, @commissionid commisionid, @agentcreditlimit agentcreditlimit, @parent_id parentid, @gparent_id gparentid      
                    --return          
                    IF @agent_status <> 'y'
                       OR @agent_status IS NULL
                        BEGIN
                            SELECT '1' code, 
                                   'agent status is ' + ISNULL(@agent_status, 'not active') message, 
                                   NULL id;
                            RETURN;
                    END;-- agent id status                            

                    IF ISNULL(@agent_lock_status, 'n') = 'y'
                        BEGIN
                            SELECT '1' code, 
                                   'agent lock status is ' + ISNULL(@agent_lock_status, 'locked') message, 
                                   NULL id;
                            RETURN;
                    END;-- agent lock/unlock                            

                    --------------if Card used------------------------------------
                    IF @card_no IS NOT NULL
                        BEGIN
                            IF NOT EXISTS
                            (
                                SELECT 'x'
                                FROM tbl_agent_card_management
                                WHERE card_no = @card_no
                                      AND ISNULL(is_active, 'n') = 'y'
                            )
                                BEGIN
                                    SELECT '1' code, 
                                           'Card doesn''t exist' message, 
                                           NULL id;
                                    RETURN;
                            END;

							SELECT @card_type_label = sd.static_data_label, 
                                   @card_type = ac.card_type,
								   --@card_amount = isnull(@card_amount,isnull(ac.amount,0))
								   @card_amount = isnull(ac.amount,0)
                            FROM tbl_agent_card_management ac
                                 JOIN tbl_static_data sd ON sd.static_data_value = ac.card_type
                            WHERE card_no = @card_no
                                  AND ISNULL(is_active, 'n') = 'y'
                                  AND sd.sdata_type_id = 23;

                    END;
                    IF @card_type = 2
                        BEGIN
                            UPDATE tbl_agent_card_management
                              SET 
                                  is_active = 'n'
                            WHERE card_no = @card_no
                                 -- AND agent_id = @agent_id;
                            SELECT '0' code, 
                                   'Gift Card Successfully Used' message, 
                                   NULL id;
								 
								 set @txn_amount = case when @amount > isnull(@card_amount,0) then  @amount - isnull(@card_amount,0) else 0 end

								 print @txn_amount

                          
                    END;
                    IF @card_type = 4
                        BEGIN
                            UPDATE tbl_agent_card_management
                              SET 
                                  Amount = Amount - @amount
                            WHERE card_no = @card_no
                                --  AND agent_id = @agent_id;
                            SELECT '0' code, 
                                   'Prepaid Card Successfully Used for Amount: ' + CAST(@amount AS VARCHAR) + ' NPR' message, 
                                   NULL id;
                            set @txn_amount =  0;
                    END;
                    --------------Card section ENd-----------------

                    IF @agent_balance < @amount
                        BEGIN
                            SELECT '1' code, 
                                   'User Balance is less than Topup Amount' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF @user_type <> 'walletUser'
                        BEGIN
                            IF @agent_type <> 'merchant'
                               AND @agent_type <> 'wallet'
                               AND @agent_type <> 'walletuser'
                               OR @agent_type IS NULL
                                BEGIN
                                    SELECT '1' code, 
                                           'agent not allowed to do transaction' message, 
                                           NULL id;
                                    RETURN;
                            END;
                    END;

                    -- only merchant or wallet allowed to inititate txn                            
                    --get parent details                      

                    SELECT @parent_status = agent_status, 
                           @parent_lock_status = lock_status, 
                           @parent_auto_commission = ISNULL(is_auto_commission, 1), 
                           @parent_commission_id = agent_commission_id, 
                           @parent_credit_limit = ISNULL(agent_credit_limit, 0)
                    FROM tbl_agent_detail
                    WHERE agent_id = @parent_id;

                    --get gparent details                            
                    SELECT @grand_parent_status = agent_status, 
                           @gparent_lock_status = lock_status, 
                           @gparent_auto_commission = ISNULL(is_auto_commission, 1), 
                           @gparent_commission_id = agent_commission_id, 
                           @gparent_credit_limit = ISNULL(agent_credit_limit, 0)
                    FROM tbl_agent_detail
                    WHERE agent_id = @gparent_id;

                    --select @gparentstatus gparentstatus,@gparentlockstatus gparentlockstatus,@gparentautocommission gparentacommission,@gparentcommissionid gparentcommid,@gparentcreditlimit gparenitclimit          
                    --return          
                    IF @parent_status IS NOT NULL
                       AND @parent_status <> 'y'
                        BEGIN
                            SELECT '1' code, 
                                   'distributor or sub distributor status is ' + ISNULL(@parent_status, 'not active') message, 
                                   NULL id;
                            RETURN;
                    END;-- parent agent status                            

                    IF @gparent_id IS NOT NULL
                        BEGIN
                            IF @grand_parent_status IS NULL
                               OR @grand_parent_status <> 'y'
                                BEGIN
                                    SELECT '1' code, 
                                           'distributor status is  ' + ISNULL(@grand_parent_status, 'not active') message, 
                                           NULL id;
                                    RETURN;
                            END;
                    END;-- if grandparent exists and status                            
                    --------------get transaction detail                            

                    SELECT @txn_type = txn_type_id, 
                           @company_id = company_id, 
                           @product = product_label, 
                           @pgateway_id = primary_gateway, 
                           @sgateway_id = secondary_gateway, 
                           @product_status = STATUS
                    FROM tbl_manage_services
                    WHERE product_id = @product_id;

                    --select @txn_type txntype, @company_id companyid, @product product, @pgatewayid pgatewayid, @sgatewayid sgatewayid,@productstatus prostatus          
                    --return          
                    IF @product_status <> 'y'
                       OR @product_status IS NULL
                        BEGIN
                            SELECT '1' code, 
                                   'product status is not active' message, 
                                   NULL id;
                            RETURN;
                    END;--product status check                            

                    IF @txn_type IS NULL
                       OR @company_id IS NULL
                       OR @product IS NULL
                        BEGIN
                            SELECT '1' code, 
                                   'product not found for processing' message, 
                                   NULL id;
                            RETURN;
                    END;--txntype/company/product is null                            

                    SET @gateway_id = ISNULL(@pgateway_id, @sgateway_id);

                    --print @gateway_id                          
                    IF NOT EXISTS
                    (
                        SELECT gateway_id
                        FROM tbl_gateway_detail
                        WHERE gateway_id = @gateway_id
                              AND gateway_status = 'y'
                    ) -- gateway status                            
                        BEGIN
                            SELECT '1' code, 
                                   'primary gateway blocked for the respective product ' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF @gateway_id IS NULL --gateway id null                            
                        BEGIN
                            SELECT '1' code, 
                                   'gateway not found for the request product' message, 
                                   NULL id;
                            RETURN;
                    END;
                    IF EXISTS
                    (
                        SELECT denomination_id
                        FROM tbl_product_denomination
                        WHERE product_id = @product_id
                              AND denomination_status = 'y'
                    ) -- if denomination exists                            
                        BEGIN
                            IF NOT EXISTS
                            (
                                SELECT denomination_id
                                FROM tbl_product_denomination
                                WHERE product_id = @product_id
                                      AND denomination_status = 'y'
                                      AND amount = @amount
                            )
                                BEGIN
                                    SELECT '1' code, 
                                           'requested amount not allowed' message, 
                                           NULL id;
                                    RETURN;
                            END;
                    END;
                    IF @txn_type = 1 -- mobiletopup                            
                        BEGIN
                            IF LEN(@subscriber_no) <> 10 -- mobile number should be 10 digits                            
                                BEGIN
                                    SELECT '1' code, 
                                           'subscription number not valid' message, 
                                           NULL id;
                                    RETURN;
                            END;
                            IF @product_id = 1 --ntc/ number validation check                            
                                BEGIN
                                    IF LEFT(@subscriber_no, 3) <> '984'
                                       AND LEFT(@subscriber_no, 3) <> '986'
                                       AND LEFT(@subscriber_no, 3) <> '985'
                                        BEGIN
                                            SELECT '1' code, 
                                                   'subscription number not valid' message, 
                                                   NULL id;
                                            RETURN;
                                    END;
                            END;
                            IF @product_id = 2 --ncell/number validatoin check                    
                                BEGIN
                                    IF(@subscriber_no <> 8800000238)
                                        BEGIN
                                            IF LEFT(@subscriber_no, 3) <> '980'
                                               AND LEFT(@subscriber_no, 3) <> '981'
                                               AND LEFT(@subscriber_no, 3) <> '982'
                                                BEGIN
                                                    SELECT '1' code, 
                                                           'subscription number not valid' message, 
                                                           NULL id;
                                                    RETURN;
                                            END;
                                    END;
                            END;
                    END;-- check mobile number mpos                            

                    SELECT @admin_rate_type = gp.commission_type, 
                           @gateway_id = pg.gateway_id, 
                           @gateway_balance = pg.gateway_balance, 
                           @txn_type = pm.txn_type_id
                    FROM tbl_manage_services pm WITH(NOLOCK)
                         JOIN tbl_gateway_detail pg WITH(NOLOCK) ON pm.primary_gateway = pg.gateway_id
                         JOIN tbl_gateway_products gp WITH(NOLOCK) ON gp.gateway_id = pg.gateway_id
                    WHERE pm.product_id = @product_id;

                    --select @admin_rate_type admincommtype, @gateway_id gateway_id, @gateway_balance gatewaybalance, @txn_type txntype,  @product_id product_id                            
                    --return                            
                    IF @admin_rate_type = 'p' -- rate in %                                            
                        BEGIN
                            SELECT @admin_comm_rate = (ISNULL(gp.commission_value, 0) / 100)
                            FROM tbl_manage_services pm WITH(NOLOCK)
                                 JOIN tbl_gateway_detail pg WITH(NOLOCK) ON ISNULL(pm.primary_gateway, pm.secondary_gateway) = pg.gateway_id
                                 JOIN tbl_gateway_products gp WITH(NOLOCK) ON gp.gateway_id = pg.gateway_id
                            WHERE pm.product_id = @product_id;

                            -----admin cost amount after discount calculation -------------------------                                                
                            SET @cost_amount_admin = ISNULL(@amount, 0) - (ISNULL(@amount, 0) * (@admin_comm_rate));        
                            -----admin cost amount after discount calculation -------------------------                                              
                            ---admin commission earned                                            
                            SET @admin_commission_earned = ISNULL(@amount, 0) - ISNULL(@cost_amount_admin, 0);        
                            --select @admin_comm_rate admincommrate,  @cost_amount_admin costamountadmin, @admin_commission_earned admincommearned                            
                            --return                            
                    END;
                    IF @admin_rate_type = 'f' -- flat rate                                            
                        BEGIN
                            SELECT @admin_comm_rate = (ISNULL(gp.commission_value, 0))
                            FROM tbl_manage_services pm WITH(NOLOCK)
                                 JOIN tbl_gateway_detail pg WITH(NOLOCK) ON ISNULL(pm.primary_gateway, pm.secondary_gateway) = pg.gateway_id
                                 JOIN tbl_gateway_products gp WITH(NOLOCK) ON gp.gateway_id = pg.gateway_id
                            WHERE pm.product_id = @product_id;

                            -----admin cost amount after discount calculation -------------------------                                                 
                            SET @cost_amount_admin = ISNULL(@amount, 0) - ISNULL(@admin_comm_rate, 0);        
                            -----admin cost amount after discount calculation -------------------------                                              
                            ---admin commission earned                                            
                            SET @admin_commission_earned = ISNULL(@amount, 0) - ISNULL(@cost_amount_admin, 0);        
                            --------                                            
                    END;
                    IF @parent_id IS NOT NULL
                        BEGIN
                            SELECT @pdist_comm_rate = (ISNULL(c.commission_value, 0) / 100), 
                                   @pdist_rate_type = ISNULL(r.is_auto_commission, 1)
                            FROM dbo.tbl_agent_detail r WITH(NOLOCK)
                                 JOIN dbo.tbl_commission_category_detail c WITH(NOLOCK) ON r.agent_commission_id = c.com_category_id
                            WHERE r.agent_id = @parent_id
                                  AND c.product_id = @product_id;
                    END;
                    IF @gparent_id IS NOT NULL
                        BEGIN
                            SELECT @gdist_comm_rate = (ISNULL(c.commission_value, 0) / 100), 
                                   @gdist_rate_type = ISNULL(r.is_auto_commission, 1)
                            FROM dbo.tbl_agent_detail r WITH(NOLOCK)
                                 JOIN dbo.tbl_commission_category_detail c WITH(NOLOCK) ON r.agent_commission_id = c.com_category_id
                            WHERE r.agent_id = @gparent_id
                                  AND c.product_id = @product_id;
                    END;

                    --select @pdistcommrate pdistcommrate, @pdistratetype distratetype, @gdistcommrate gdistcomrate, @gdistratetype gdistratetype          
                    -- for distributor itself  / no parent                                    
                    SELECT @dist_comm_rate = (ISNULL(c.commission_value, 0) / 100), 
                           @dist_rate_type = ISNULL(r.is_auto_commission, 1)
                    FROM dbo.tbl_agent_detail r WITH(NOLOCK)
                         JOIN dbo.tbl_commission_category_detail c WITH(NOLOCK) ON r.agent_commission_id = c.com_category_id
                    WHERE r.agent_id = @agent_id
                          AND c.product_id = @product_id;

                    ----------------------- agent/user selling price calculation ---------------------                                                                        
                    SELECT @category_id = com_category_id, 
                           @merchant_rate = (ISNULL(commission_value, 0) / 100), 
                           @is_auto_commission = ISNULL(b.is_auto_commission, 1)
                    FROM tbl_agent_detail b WITH(NOLOCK)
                         LEFT OUTER JOIN dbo.tbl_commission_category_detail c WITH(NOLOCK) ON c.com_category_id = b.agent_commission_id
                    WHERE b.agent_id = @agent_id
                          AND c.product_id = @product_id;

                    --select @com_category_id com_category_id, @merchant_rate merchantrate, @is_auto_commission atocommision          
                    --return          
                    ----------------------------------------------------------------------------------                                             
                    -----selling price of the agent with  assigned discount rate                                                                              
                    SET @cost_amount = ISNULL(@amount, 0) - (ISNULL(@amount, 0) * (@merchant_rate));

                    -------------------------------------------------------------------                                              
                    -------calculate distributor cashback after agent's cashback deduction -----------                                        
                    IF @parent_id IS NULL
                       AND @gparent_id IS NULL
                        BEGIN
                            SET @dist_comm_earned = ISNULL(@cost_amount, 0) - ISNULL(@dist_cost_amount, 0);        
                            -----    reseller cost amount after discount calculation -------------------------                                                          
                            SET @dist_cost_amount = ISNULL(@amount, 0) - (ISNULL(@amount, 0) * (@dist_comm_rate));
                    END;
                    IF @parent_id IS NOT NULL
                        BEGIN
                            SET @pdist_cost_amount = ISNULL(@amount, 0) - (ISNULL(@amount, 0) * (@pdist_comm_rate));
                            SET @pdist_comm_earned = ISNULL(@cost_amount, 0) - ISNULL(@pdist_cost_amount, 0);
                    END;
                    IF @gparent_id IS NOT NULL
                        BEGIN
                            SET @gdist_cost_amount = ISNULL(@amount, 0) - (ISNULL(@amount, 0) * (@gdist_comm_rate));
                            SET @gdist_comm_earned = ISNULL(@cost_amount, 0) - ISNULL(@gdist_cost_amount, 0);
                    END;

                    --  -------- user cashback amount ---------------------------------------                                                    
                    SET @merchant_comm_earned = (ISNULL(@amount, 0) - ISNULL(@cost_amount, 0));

                    --  -----------------------------------------------                                    
                    --deduct merchant account                                                                                                           
                    UPDATE dbo.tbl_agent_detail
                      SET 					  
                          available_balance = case when @txn_amount is not null then
						  ISNULL(available_balance, 0) - ISNULL(@txn_amount, 0)
						  else  ISNULL(available_balance, 0) - ISNULL(@amount, 0) end
                      WHERE agent_id = @agent_id;

                    --select @cost_amount_admin, @gateway_id
                    --return
                    --deduct gateway balance                                            
                    UPDATE dbo.tbl_gateway_detail
                      SET 
                          gateway_balance = ISNULL(gateway_balance, 0) - ISNULL(@cost_amount_admin, 0)
                    WHERE gateway_id = @gateway_id;

                    --log last client balance                                                                                             
                    SELECT @last_client_balance = ISNULL(available_balance, 0)
                    FROM dbo.tbl_agent_detail WITH(NOLOCK)
                    WHERE agent_id = @agent_id;

                    -- insert transaction detail into transaction table                            
                    INSERT INTO [dbo].tbl_transaction
                    ([product_id], 
                     [product_label], 
                     [txn_type_id], 
                     company_id,         
                     -- grand_parent_id,         
                     parent_id, 
                     [agent_id], 
                     subscriber_no, 
                     [amount], 
                     service_charge, 
                     bonus_amt, 
                     [status], 
                     [user_id], 
                     created_UTC_date, 
                     created_local_date, 
                     created_nepali_date, 
                     created_by, 
                     created_ip, 
                     created_platform, 
                     [gateway_id], 
                     is_auto_commission_agent, 
                     is_auto_commission_parent, 
                     is_auto_commission_gparent, 
                     admin_commission, 
                     last_agent_balance, 
                     agent_remarks, 
                     admin_cost_amount, 
                     process_id
                    )
                    VALUES
                    (@product_id, 
                     @product, 
                     @txn_type, 
                     @company_id,         
                     -- @gparent_id,         
                     @parent_id, 
                     @agent_id, 
                     @subscriber_no, 
                     @amount, 
                     ISNULL(@service_charge, 0), 
                     ISNULL(@bonus_amt, 0), 
                     'Pending', 
                     @user_id, 
                     GETUTCDATE(), 
                     GETDATE(), 
                     [dbo].func_get_nepali_date(DEFAULT), 
                     @action_user, 
                     @created_ip, 
                     @created_platform, 
                     @gateway_id, 
                     @dist_rate_type, 
                     @pdist_rate_type, 
                     @gdist_rate_type, 
                     @admin_commission_earned, 
                     @last_client_balance, 
                     'Transaction is Pending', 
                     @cost_amount_admin, 
                     @process_id
                    );
                    SET @txn_id = SCOPE_IDENTITY();

                    -- insert transaction commission detail into commission table                         
                    INSERT INTO [dbo].tbl_transaction_commission
                    ([txn_id], 
                     [agent_id], 
                     [agent_commission], 
                     [parent_commission], 
                     [grand_parent_commission], 
                     txn_reward_point
                    )
                    VALUES
                    (@txn_id, 
                     @agent_id, 
                     ISNULL(@merchant_comm_earned, 0), 
                     ISNULL(@pdist_comm_earned, 0), 
                     ISNULL(@gdist_comm_earned, 0), 
                     0
                    );


					--- if Card used insert into card transction detail table
					if @card_no is not null
					Begin					
						insert into tbl_transaction_detail_cards
						(
							txn_id ,
							Card_no ,
							Card_Type_id ,
							Card_type ,
							Card_amount ,
							created_by ,
							created_local_date,
							created_utc_date ,
							created_nepali_date					
						)
						values
						(
							@txn_id,
							@card_no,
							@card_type,
							@card_type_label,
							case 
							when @card_type=2 then isNull(@card_amount,0) 
							when @card_type=4 then isNull(@amount,0) end,
							@action_user,
							GETDATE(),
							GETUTCDATE(),
							dbo.func_get_nepali_date(default)					
						)
					End

                    SELECT '0' code, 
                           'Transaction is pending for subscriber no: ' + @subscriber_no + ' for amount: ' + CAST(@amount AS VARCHAR) message, 
                           @txn_id tranno, 
                           GETDATE() txn_date;
            END;
            IF @flag = 's'
                BEGIN
                    IF NOT EXISTS
                    (
                        SELECT 'x'
                        FROM tbl_transaction dt
                             LEFT JOIN tbl_transaction_detail dtd ON dtd.txn_id = dt.txn_id
                        WHERE dt.txn_id = @txn_id
                    )
                        BEGIN
                            SELECT '1' code, 
                                   'error when checking transaction. please contact support.' message;
                            RETURN;
                    END;
                    SELECT @product_id = product_id
                    FROM tbl_transaction
                    WHERE txn_id = @txn_id;
                    IF(@product_id = '21')
                        BEGIN
                            SELECT TOP 1 '0' code, 
                                         gd.gateway_username, 
                                         gd.gateway_password, 
                                         dt.amount, 
                                         fd.contact_name, 
                                         fd.contact_email, 
                                         fd.adult_passenger adult, 
                                         fd.child_passenger child, 
                                         fd.tax_currency currency, 
                                         fd.contact_phone contactno, 
                                         dt.product_id, 
                                         fd.inbound_flight_id, 
                                         fd.outbound_flight_id, 
                                         fd.nationality, 
                                         dt.txn_id AS partnertxnid, 
                                         dt.created_by, 
                                         dtd.extra_field1, 
                                         dtd.extra_field2, 
                                         dtd.extra_field3, 
                                         dtd.extra_field4, 
                                         dt.gateway_id
                            FROM tbl_transaction dt
                                 LEFT JOIN tbl_transaction_detail dtd ON dtd.txn_id = dt.txn_id
                                 LEFT JOIN tbl_transaction_detail_flight_detail fd ON fd.txn_id = dt.txn_id
                                 LEFT JOIN tbl_gateway_detail gd ON gd.gateway_id = dt.gateway_id
                            WHERE dt.txn_id = @txn_id;
                            SELECT '0' code, 
                                   pax_type AS passengertype, 
                                   title, 
                                   first_name, 
                                   last_name
                            FROM tbl_transaction_detail_flight_detail
                            WHERE txn_id = @txn_id;
                            RETURN;
                    END;
                    IF @product_id IN(20, 36, 50, 59, 60, 64, 65)
                        BEGIN
                            SELECT '0' code, 
                                   dtd.extra_field1, 
                                   dtd.extra_field2, 
                                   dtd.extra_field3, 
                                   dtd.extra_field4, 
                                   dt.user_id, 
                                   dt.created_by AS actionuser, 
                                   dt.created_ip AS ip_address, 
                                   dtd.policy_holder_name, 
                                   dtd.insurance_policy_number, 
                                   dtd.insurance_document_number, 
                                   dtd.date_of_birth, 
                                   dtd.extra_field5, 
                                   dtd.extra_field6, 
                                   dtd.extra_field7, 
                                   gd.gateway_username, 
                                   gd.gateway_password, 
                                   dt.amount, 
                                   dt.subscriber_no, 
                                   dt.gateway_id, 
                                   dt.product_id
                            FROM tbl_transaction dt
                                 LEFT JOIN tbl_insurance_detail dtd ON dtd.txnid = dt.txn_id
                                 LEFT JOIN tbl_gateway_detail gd ON gd.gateway_id = dt.gateway_id
                            WHERE dt.txn_id = @txn_id;
                    END;
                    SELECT '0' code, 
                           gd.gateway_username, 
                           gd.gateway_password, 
                           dt.amount, 
                           dt.subscriber_no, 
                           dt.product_id, 
                           dt.created_by, 
                           dtd.extra_field1, 
                           dtd.extra_field2, 
                           dtd.extra_field3, 
                           dtd.extra_field4, 
                           dt.gateway_id
                    FROM tbl_transaction dt
                         LEFT JOIN tbl_transaction_detail dtd ON dtd.txn_id = dt.txn_id
                         LEFT JOIN tbl_gateway_detail gd ON gd.gateway_id = dt.gateway_id
                    WHERE dt.txn_id = @txn_id;
            END;
            IF @flag = 'gty'
                BEGIN
                    IF @product_id IS NOT NULL
                        BEGIN
                            SELECT '0' code, 
                                   gd.gateway_username, 
                                   gd.gateway_password, 
                                   gd.gateway_id, 
                                   gd.gateway_url, 
                                   gd.gateway_api_token, 
                                   sd.additional_value1 gtyrepo
                            FROM tbl_gateway_detail gd
                                 JOIN tbl_manage_services ms ON ms.primary_gateway = gd.gateway_id
                                 LEFT JOIN tbl_static_data sd ON sd.static_data_row_id = 14
                                                                 AND sd.static_data_value = ms.product_id
                            WHERE ms.product_id = @product_id;
                            RETURN;
                    END;
                        ELSE
                        IF @extra_field1 IS NOT NULL
                            BEGIN
                                SELECT '0' code, 
                                       gd.gateway_username, 
                                       gd.gateway_password, 
                                       gd.gateway_id, 
                                       gd.gateway_url, 
                                       gd.gateway_api_token
                                FROM tbl_gateway_detail gd
                                WHERE gd.gateway_name LIKE '%' + @extra_field1 + '%'
                                      AND gateway_status = 'y';
                        END;
                    SELECT '0' code, 
                           gd.gateway_username, 
                           gd.gateway_password, 
                           gd.gateway_id
                    FROM tbl_gateway_detail gd
                    WHERE gd.gateway_id = @gateway_id;
            END;
        END TRY
        BEGIN CATCH
            IF @@trancount > 0
                ROLLBACK TRANSACTION;
            SELECT 1 code, 
                   ERROR_MESSAGE(), 
                   ERROR_LINE() message, 
                   NULL id;
        END CATCH;
    END;   
GO


