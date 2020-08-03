USE [WePayNepal]
GO

/****** Object:  StoredProcedure [dbo].[sproc_txn_response]    Script Date: 6/4/2020 5:45:48 PM ******/
DROP PROCEDURE [dbo].[sproc_txn_response]
GO

/****** Object:  StoredProcedure [dbo].[sproc_txn_response]    Script Date: 6/4/2020 5:45:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[sproc_txn_response] @product_id       INT            = NULL, 
                                                     @subscriber_no    VARCHAR(100)   = NULL, 
                                                     @amount           DECIMAL(18, 2)  = NULL, 
                                                     @agent_id         INT            = NULL, 
                                                     @status           VARCHAR(20)    = NULL, 
                                                     @user_id          VARCHAR(20)    = NULL, 
                                                     @created_ip       VARCHAR(10)    = NULL, 
                                                     @created_platform VARCHAR(200)   = NULL, 
                                                     @updated_by       VARCHAR(20)    = NULL, 
                                                     @process_id       VARCHAR(100)   = NULL, 
                                                     @service_charge   DECIMAL(18, 2)  = NULL, 
                                                     @txn_id           VARCHAR(30)    = NULL, 
                                                     @additional_data  NVARCHAR(MAX)  = NULL, 
                                                     @action_user      VARCHAR(100)   = NULL, 
                                                     @remarks          VARCHAR(1000)  = NULL, 
                                                     @gtw_response     VARCHAR(MAX)   = NULL, 
                                                     @gtw_txn_id       VARCHAR(100)   = NULL, 
                                                     @gtw_bill_id      VARCHAR(100)   = NULL, 
                                                     @partner_txn_id   VARCHAR(30)    = NULL, 
                                                     @from_ip_address  VARCHAR(20)    = NULL, 
                                                     @agent_response   VARCHAR(500)   = NULL, 
                                                     @mobile_number    VARCHAR(20)    = NULL, 
                                                     @customer_name    VARCHAR(200)   = NULL, 
                                                     @customer_id      VARCHAR(20)    = NULL, 
                                                     @plan_id          VARCHAR(20)    = NULL, 
                                                     @plan_name        VARCHAR(500)   = NULL
AS
    BEGIN
        -- set nocount on added to prevent extra result sets from          
        -- interfering with select statements.          
        SET NOCOUNT ON;
        DECLARE @agent_commission FLOAT, @parent_commission FLOAT, @gparent_commission FLOAT, @parent_id INT, @gparent_id INT, @parent_commission_type CHAR(3), @gparent_commission_type CHAR(3), @agent_commission_type CHAR(3), @agent_name VARCHAR(50), @parent_name VARCHAR(50), @grand_parent_name VARCHAR(50), @admin_cost_amount DECIMAL(18, 2), @txn_status VARCHAR(100), @product_type VARCHAR(100), @gateway_id INT, @gateway_balance DECIMAL(18, 2), @card_no VARCHAR(16), @card_amount DECIMAL(18, 2), @card_type_id INT;
        IF NULLIF(@agent_response, '') IS NULL
            SET @agent_response = @remarks;
        IF NULLIF(@user_id, '') IS NULL
            --SET @user_id = @action_user;
			select @user_id = user_id from tbl_user_detail where user_name = @action_user ;
		IF @status is null
			SET @status = 1;

        IF @status = '000'
           OR @status = '0'
            BEGIN
                IF EXISTS
                (
                    SELECT 'x'
                    FROM tbl_transaction WITH(NOLOCK)
                    WHERE STATUS IN('success', 'fail')
                    AND txn_id = @txn_id
                )
                    BEGIN
                        SELECT '1' code, 
                               message = 'transaction status is already updated.', 
                               NULL id;
                        RETURN;
                END;
                SET @txn_status = 'success';

                ---success case                           
                SELECT @product_id = dt.product_id, 
                       @product_type = ms.product_type
                FROM tbl_transaction dt
                     JOIN tbl_manage_services ms ON ms.product_id = dt.product_id
                WHERE txn_id = @txn_id;
                IF @product_type = 'rcpin'
                    BEGIN
                        CREATE TABLE #tempbooking
                        ([code]       VARCHAR(5), 
                         [message]    VARCHAR(100), 
                         [serialno]   VARCHAR(150), 
                         [pinno]      VARCHAR(150), 
                         [expirydate] DATETIME
                        );
                        INSERT INTO #tempbooking
                        ([code], 
                         [message], 
                         [serialno], 
                         [pinno], 
                         [expirydate]
                        )
                        EXEC sproc_epin_parse_detail 
                             @additional_data, 
                             @action_user, 
                             @txn_id, 
                             @created_ip, 
                             @user_id, 
                             @gtw_txn_id, 
                             @amount, 
                             @product_id;
                        IF
                        (
                            SELECT code
                            FROM #tempbooking
                        ) <> '0'
                            BEGIN
                                SELECT code, 
                                       [message]
                                FROM #tempbooking;
                                RETURN;
                        END;
                END;
                IF @product_id = 21
                    BEGIN
                        EXEC sproc_parse_air_ticket_detail 
                             'u', 
                             @additional_data, 
                             @action_user, 
                             @txn_id, 
                             @created_ip, 
                             @user_id;
                END;
                IF @product_id = 6
                    BEGIN
                        EXEC sproc_parse_movie_ticket_detail 
                             'u', 
                             @additional_data, 
                             @action_user, 
                             @txn_id, 
                             @created_ip, 
                             @user_id;
                END;
                UPDATE dbo.tbl_transaction
                  SET 
                      admin_remarks = @gtw_response, 
                      STATUS = @txn_status, 
                      gateway_txn_id = @gtw_txn_id, 
                      gateway_bill_id = @gtw_bill_id, 
                      updated_local_date = GETDATE(), 
                      updated_UTC_date = GETUTCDATE(), 
                      updated_nepali_date = [dbo].func_get_nepali_date(DEFAULT), 
                      updated_ip = @from_ip_address, 
                      updated_by = @user_id, 
                      agent_remarks = 'Transaction is Succesfull', 
                      status_code = @status, 
                      json_data = @additional_data, 
                      partner_txn_id = @partner_txn_id
                WHERE txn_id = @txn_id;
                UPDATE tbl_transaction_detail
                  SET 
                      customer_id = ISNULL(@customer_id, customer_id), 
                      customer_name = ISNULL(@customer_name, customer_name), 
                      invoice_id = @gtw_txn_id, 
                      plan_id = ISNULL(@plan_id, plan_id), 
                      plan_name = ISNULL(@plan_name, plan_name)
                WHERE txn_id = @txn_id;

                -- get transaction details          
                SELECT @agent_commission = agent_commission, 
                       @parent_commission = parent_commission, 
                       @gparent_commission = grand_parent_commission, 
                       @agent_id = t.agent_id, 
                       @parent_id = t.parent_id, 
                       @gparent_id = t.grand_parent_id, 
                       @parent_commission_type = dd.is_auto_commission, 
                       @user_id = t.user_id
                       ,
                       --@gparent_commission_type = pdd.is_auto_commission,    
                       @agent_commission_type = ad.is_auto_commission, 
                       @agent_name = ad.full_name, 
                       @subscriber_no = t.subscriber_no, 
                       @parent_name = dd.full_name,
                       --@grand_parent_name = pdd.full_name,   
                       @created_platform = t.created_platform, 
                       @product_id = product_id, 
                       @process_id = process_id, 
                       @gateway_id = gateway_id
                FROM tbl_transaction t WITH(NOLOCK)
                     LEFT JOIN tbl_transaction_commission ptd ON ptd.txn_id = t.txn_id
                     LEFT JOIN tbl_transaction_detail td ON td.txn_detail_id = t.txn_id
                     JOIN tbl_agent_detail ad ON ad.agent_id = t.agent_id
                     LEFT OUTER JOIN tbl_agent_detail dd ON dd.parent_id = t.parent_id
                --left outer join tbl_agent_detail pdd on pdd.grand_parent_id = t.grand_parent_id  
                WHERE t.txn_id = @txn_id;
                IF @product_id IN(20, 36, 50, 59, 60, 64, 65)
                    BEGIN
                        UPDATE tbl_insurance_detail
                          SET 
                              txnid = @txn_id, 
                              updated_by = @action_user, 
                              updated_local_date = GETDATE(), 
                              updated_UTC_date = GETUTCDATE(), 
                              updated_ip = @from_ip_address, 
                              updated_nepali_date = [dbo].func_get_nepali_date(DEFAULT)
                        WHERE process_id = @process_id;
                END;

                --insert/update merchant commission          
                IF ISNULL(@agent_commission_type, 'm') = 'a'
                    BEGIN
                        IF ISNULL(@agent_commission, 0) > 0
                            BEGIN
                                ----add to merchant wallet -----------------------------------------                                  
                                UPDATE dbo.tbl_agent_detail
                                  SET 
                                      available_balance = available_balance + ISNULL(@agent_commission, 0)
                                WHERE agent_id = @agent_id;
                                
                        END;
                                --   ------------------------ ---------------------------------------                                    
                END;
                    ELSE
                    IF @parent_id IS NOT NULL
                        BEGIN
                            --  ---********************* parent distributor *********_----------------------------------------------------------                             
                            --  -------------------------------------------------------------------------------                      
                            IF ISNULL(@parent_commission_type, 'm') = 'a'
                                BEGIN
                                    IF ISNULL(@parent_commission, 0) > 0
                                        BEGIN
                                            ------------add to distributor wallet ----------------------                                  
                                            UPDATE tbl_agent_detail
                                              SET 
                                                  available_balance = ISNULL(available_balance, 0) + ISNULL(@parent_commission, 0)
                                            WHERE agent_id = @parent_id;
                                    END;
                            END;

                            --   ----------------------------------------------------------                                
                            -- ---------------------------sub distributor----------------------------------------------------                                  
                            ------------add to sub_distributor/parent distributor wallet ----------------------                
                            IF @gparent_id IS NOT NULL
                                BEGIN
                                    IF ISNULL(@gparent_commission_type, 'm') = 'a'
                                        BEGIN
                                            IF ISNULL(@gparent_commission, 0) > 0
                                                BEGIN
                                                    UPDATE tbl_agent_detail
                                                      SET 
                                                          available_balance = ISNULL(available_balance, 0) + ISNULL(@parent_commission, 0)
                                                    WHERE agent_id = @gparent_id;
                                            END;
                                    END;
                            END;
                    END;
                IF @product_type = 'rcpin'
                    BEGIN
                        SELECT *
                        FROM #tempbooking;
                        RETURN;
                END;

                --deduct gateway amount          
                UPDATE dbo.tbl_gateway_detail
                  SET 
                      gateway_balance = ISNULL(gateway_balance, 0) - ISNULL(@admin_cost_amount, 0)
                WHERE gateway_id = @gateway_id;
                SELECT @status code, 
                       'transaction updated successfully' message, 
                       @txn_id txnid, 
                       @gtw_txn_id gtwayid;

                --select @agent_id, @amount, @subscriber_no, @txn_id, @user_id, @agent_id
                --return

                INSERT INTO tbl_agent_notification
                (agent_id, 
                 notification_body, 
                 user_id, 
                 notification_subject, 
                 notification_to, 
                 created_by, 
                 created_local_date, 
                 created_UTC_date, 
                 txn_id, 
                 notification_type, 
                 notification_status,
				 read_status,
				 txn_status_id,
				 txn_status
                )
                VALUES
                (@agent_id, 
                 'Transaction Successful for ' + CAST(@amount AS VARCHAR) + ' on Subscriber No: ' + @subscriber_no + ': TxnId: ' + CAST(@txn_id AS VARCHAR), 
                 @user_id, 
                 'Mobile Topup', 
                 @agent_id, 
                 @updated_by, 
                 GETDATE(), 
                 GETUTCDATE(), 
                 @txn_id, 
                 'Top up', 
                 'y',
				 'n',
				 @status,
				 @txn_status
                );
                RETURN;
        END;
            ELSE
            IF @status = '777'
               OR @status = '778'
                BEGIN
                    SET @txn_status = 'pending';
                    SELECT @status code, 
                           'transaction status is pending. please note your transaction id' + ' and contact support. your transaction id is ' + CAST(@txn_id AS VARCHAR) message, 
                           @txn_id txnid, 
                           @gtw_txn_id gtwayid;
                    RETURN;
            END;
                ELSE
                IF @status != '000'
                   AND @status != '0'
                    BEGIN
                        IF EXISTS
                        (
                            SELECT 'x'
                            FROM tbl_agent_balance
                            WHERE txn_id = @txn_id
                                  AND txn_type = 'r'
                        )
                            BEGIN
                                SELECT '1' code, 
                                       'this transaction is already refunded' message, 
                                       NULL id;
                                RETURN;
                        END;
                        SET @txn_status = 'failed';

                        --fail case                                                                                
                        UPDATE dbo.tbl_transaction
                          SET 
                              admin_remarks = @remarks, 
                              STATUS = @txn_status, 
                              gateway_txn_id = @gtw_txn_id, 
                              updated_local_date = GETDATE(), 
                              updated_UTC_date = GETUTCDATE(), 
                              updated_nepali_date = [dbo].func_get_nepali_date(DEFAULT), 
                              updated_ip = @from_ip_address, 
                              updated_by = @user_id, 
                              agent_remarks = @agent_response, 
                              status_code = @status
                        WHERE txn_id = @txn_id;

                        ------------------------------------------------------------------------------------          
                        -- get transaction details          
                        SELECT @agent_commission = agent_commission, 
                               @parent_commission = parent_commission, 
                               @gparent_commission = grand_parent_commission, 
                               @agent_id = t.agent_id, 
                               @parent_id = t.parent_id, 
                               @gparent_id = t.grand_parent_id, 
                               @parent_commission_type = dd.is_auto_commission,
                               --@gparent_commission_type = pdd.is_auto_commission,   
                               @agent_commission_type = ad.is_auto_commission, 
                               @agent_name = ad.full_name, 
                               @parent_name = dd.full_name,
                               --@grand_parent_name = pdd.full_name,   
                               @created_platform = t.created_platform, 
                               @admin_cost_amount = t.admin_cost_amount, 
                               @gateway_id = t.gateway_id, 
                               @amount = t.amount
                        FROM tbl_transaction t WITH(NOLOCK)
                             JOIN tbl_transaction_commission ptd ON ptd.txn_id = t.txn_id
                             LEFT JOIN tbl_transaction_detail td ON td.txn_id = t.txn_id
                             JOIN tbl_agent_detail ad ON ad.agent_id = t.agent_id
                             LEFT OUTER JOIN tbl_agent_detail dd ON dd.parent_id = t.parent_id
                        -- left outer join tbl_agent_detail pdd on pdd.grand_parent_id = t.grand_parent_id  
                        WHERE t.txn_id = @txn_id;

                        --get card details 
                        SELECT @card_no = Card_no, 
                               @card_amount = Card_amount, 
                               @card_type_id = Card_Type_id
                        FROM tbl_transaction_detail_Cards
                        WHERE txn_id = @txn_id;


                        INSERT INTO dbo.tbl_agent_balance
                        (agent_id, 
                         agent_name, 
                         amount, 
                         currency_code, 
                         agent_remarks, 
                         user_id, 
                         txn_type, 
                         created_utc_date, 
                         created_local_date, 
                         created_nepali_date, 
                         created_by, 
                         created_ip, 
                         created_platform, 
                         agent_parent_id, 
                         txn_id
                        )
                        VALUES
                        (@agent_id, 
                         @agent_name, 
                         @amount, 
                         'npr', 
                         @remarks, 
                         @user_id, 
                         'r', 
                         GETUTCDATE(), 
                         GETDATE(), 
                         [dbo].func_get_nepali_date(DEFAULT), 
                         @action_user, 
                         @created_ip, 
                         @created_platform, 
                         @parent_id, 
                         @txn_id
                        );  
                        --refunded  transaction amount  
						set @amount = case when (@card_no is not null AND @card_type_id = 2) then (isnull(@amount,0)-isnull(@card_amount,0)) 
									       when (@card_no is not null AND @card_type_id = 4) then 0
									       else @amount
									  end
                        UPDATE dbo.tbl_agent_detail
                          SET 
                              available_balance = ISNULL(available_balance, 0) + ISNULL(@amount, 0)
                        WHERE agent_id = @agent_id;

                        --refund gateway amount          
                        UPDATE dbo.tbl_gateway_detail
                          SET 
                              gateway_balance = ISNULL(gateway_balance, 0) + ISNULL(@admin_cost_amount, 0)
                        WHERE gateway_id = @gateway_id;

                        --refund card transaction
                        IF @card_type_id = 2
                            BEGIN
                                UPDATE tbl_agent_card_management
                                  SET 
                                      is_active = 'y'
                                WHERE card_no = @card_no;
                        END;
                        IF @card_type_id = 4
                            BEGIN
                                UPDATE tbl_agent_card_management
                                  SET 
                                      Amount = ISNULL(Amount, 0) + @Card_amount
                                WHERE card_no = @card_no;
                        END;
                        SELECT @status code, 
                               'Transaction failed ' + @remarks message, 
                               NULL id;
                        RETURN;
                END;
    END;
GO


