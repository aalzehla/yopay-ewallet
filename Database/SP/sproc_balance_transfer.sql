USE [WePayNepal]
GO

/****** Object:  StoredProcedure [dbo].[sproc_balance_transfer]    Script Date: 6/5/2020 10:48:26 AM ******/
DROP PROCEDURE [dbo].[sproc_balance_transfer]
GO

/****** Object:  StoredProcedure [dbo].[sproc_balance_transfer]    Script Date: 6/5/2020 10:48:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



       
CREATE            PROCEDURE [dbo].[sproc_balance_transfer] @flag             CHAR(5),             
                                                @agent_id         INT            = NULL,             
                                                @amount           DECIMAL(18, 2)  = NULL,             
                                                @remarks          VARCHAR(500)   = NULL,             
                                                @bank_id          VARCHAR(10)    = NULL,             
                                                @bank_name        VARCHAR(100)   = NULL,             
                                                @type             CHAR(3)        = NULL,             
                                                @action_user      VARCHAR(20)    = NULL,             
                                                @created_ip       VARCHAR(20)    = NULL,             
                                                @from_date        DATETIME       = NULL,             
                                                @to_date          DATETIME       = NULL,             
                                                @created_platform VARCHAR(50)    = NULL,             
                                                @scharge          DECIMAL(18, 2)  = NULL,             
                                                @bonus_amt        DECIMAL(18, 2)  = NULL,             
                                                @sender_id        INT            = NULL,             
                                                @reciever_id      INT            = NULL,             
                                                @grand_parent_id    INT            = NULL,             
                                                @parent_id        INT            = NULL,             
                                                @subscriber       VARCHAR(100)   = NULL,            
            @balance_id int = null,  
            @description varchar(500) =  NUll,  
            @bt_purpose varchar(60) =  null,  
            @user_name varchar(50) =  null  
AS            
    BEGIN            
        SET NOCOUNT ON;            
 DECLARE @agent_name VARCHAR(100), @desc VARCHAR(MAX), @agent_old_balance MONEY, @sql VARCHAR(MAX), @remarks_agent VARCHAR(MAX), @agent_current_balance DECIMAL(18, 2), @subscriber_no VARCHAR(100), @txn_id INT, @sender_name VARCHAR(100)  
 DECLARE @agent_parent_Id int, @agent_parent_balance decimal(18,2), @email_id varchar(50) ,@user_id int , @user_mobile_number varchar(20), @reciever_agent_id  int ,@reciever_full_name  Varchar(50),@reciever_email varchar(50)   
    DECLARE @user_email varchar(50),@user_full_name varchar(100), @sender_agent_id int  
   
  select @agent_parent_Id =parent_id, @agent_old_balance = ISNULL(available_balance,0), @agent_name = agent_name from tbl_agent_Detail where Agent_id = @agent_id            
  select @agent_parent_balance = ISNULL(available_balance,0) from tbl_agent_Detail where  Agent_id = @agent_parent_Id            
            
  IF @amount < 0            
            BEGIN            
                SET @amount = @amount * -1;            
        END;            
            
  if @bank_id is not null            
  begin            
   select @bank_name =  funding_bank_name from tbl_funding_bank_account where funding_bank_id = @bank_id            
  end            
            
  begin try            
  if @flag = 't'            
  begin            
   begin try            
    begin transaction distributorbalancetransfer            
            
    if @agent_id is null            
    begin            
     select '1' code            
      ,'distributor id can''t be null' message            
      ,null id            
            
     return            
    end            
                
            
    if @amount < 0            
    begin            
     select '1' code            
      ,'amount can''t be less than ''0''' message            
      ,null id            
            
     return            
    end            
            
        insert into [dbo].tbl_agent_balance(            
     agent_id            
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip     
  ,txn_mode    
     )         
    values (            
     @agent_id            
     ,@agent_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip    
  ,'CR'    
     )            
            
            
     update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) + @amount            
    where agent_id = @agent_id            
            
    select '0' code            
     ,'successfully transfered balance of ' + cast(@amount as varchar) +' to distributor: ' + @agent_name message            
     ,null id            
                
    commit transaction distributorbalancetransfer            
    end try            
            
    begin catch            
    if @@trancount > 0            
     rollback transaction distributorbalancetransfer            
            
    set @desc = 'sql error found:(' + error_message() + ')'            
            
    insert into tbl_error_log_sql(            
     sql_error_desc            
     ,sql_error_script            
     ,sql_query_string            
     ,sql_error_category            
     ,sql_error_source            
     ,sql_error_local_date            
     ,sql_error_UTC_date            
     ,sql_error_nepali_date            
     )            
    select @desc            
     ,'sproc_balance_transfer(distributor balance transfer:flag ''i'')'            
     ,'sql'            
     ,'sql'            
     ,'sproc_balance_transfer(distributor balance transfer)'            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
            
    select '1' code            
     ,'errorid: ' + cast(scope_identity() as varchar) message            
     ,null id            
   end catch            
  end -- update balance for distributor by admin            
            
            
  if @flag = 'rt'            
begin            
 begin try            
  begin transaction distributorbalanceretrieve            
            
  SELECT @agent_name = ad.agent_name,            
 @agent_old_balance = isnull(available_balance, 0)            
 FROM tbl_agent_detail ad            
 join tbl_agent_balance ab on ad.agent_id  = ab.agent_id            
 WHERE ad.agent_id = @agent_id;            
            
  if @amount < 0            
  begin            
   select '1' code            
    ,'amount can''t be less  than 0' message            
    ,null id            
            
   return            
  end            
            
  if @amount > isnull(@agent_old_balance, 0)            
  begin            
   select '1' code            
    ,'distributor balance is less than the amount for retrieval' message            
    ,null id            
            
   return            
  end            
            
  insert into [dbo].tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,bank_id            
   ,bank_name            
   ,[amount]            
   ,txn_type            
   ,currency_code            
   ,agent_remarks            
   ,created_by            
   ,created_local_date            
   ,created_UTC_date            
   ,created_nepali_date            
   ,created_ip    
   ,txn_mode    
   )            
  values (            
   @agent_id            
   ,@agent_name            
   ,@bank_id            
   ,@bank_name            
   ,@amount            
   ,@type            
   ,'npr'            
   ,@remarks            
   ,@action_user            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
   ,@created_ip      
   ,'DR'    
   )            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance, 0) - @amount            
  where agent_id = @agent_id            
            
  select '0' code            
   ,'successfully retrieved balance of ' + cast(@amount as varchar) +' from distributor: ' + @agent_name message            
   ,null id            
            
  --exec spa_email_request @flag='brd', @previous_balance=@distibutoroldbalance,@current_balance=@newdisbalance, @distributor_id=@distributer_id, @amount=@amount, @bankname=@bank_name                 
  --return                
  commit transaction distributorbalanceretrieve            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction distributorbalanceretrieve            
            
  set @desc = 'sql error found:(' + error_message() + ')'            
            
  insert into tbl_error_log_sql(            
   sql_error_desc            
   ,sql_error_script            
   ,sql_query_string            
   ,sql_error_category            
   ,sql_error_source            
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(distributor balance retrieve:flag ''rt'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer(distributor balance retrieve)'            
   ,getdate()            
   ,getutcdate()          
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) + error_line() message            
   ,null id            
 end catch            
end -- retrieve distributor balance  by admin                
            
  if @flag = 'r'            
begin            
            
 set @sql = ' select balance_id, ab.agent_id, ab.agent_name, amount, CASE when ab.txn_type = UPPER(''T'')then            
    ad.available_balance - amount            
    else             
    ad.available_balance + amount            
end Agent_old_balance, fb.funding_bank_branch, fb.funding_account_number,bank_name,            
 txn_type, agent_remarks, txn_mode,            
 Isnull(ad.available_balance,0)as New_Balance,            
 ab.created_by,             
 ab.created_local_Date as Created_date,            
 ab.created_nepali_Date             
 from tbl_agent_balance ab            
 join tbl_agent_detail ad on ad.agent_id  = ab.agent_id            
 left join tbl_funding_bank_account fb on fb.funding_bank_id = ab.bank_id             
 where 1 = 1'            
            
 if @balance_id is not null            
 begin            
  set @sql += 'and balance_id= ' +Cast(@balance_id as varchar)            
 end            
 if @type is not null            
 begin            
  set @sql += ' and txn_type=' + @type            
 end            
               
            
 if @agent_id is not null            
 begin            
  set @sql += ' and ab.agent_id = ' + cast(@agent_id as varchar)            
 end            
            
 if @bank_id is not null            
 begin            
  set @sql += ' and ab.bank_id = ' + cast(@bank_id as varchar)            
 end            
            
               
 if @action_user is not null            
 begin            
  set @sql += ' and ab.created_by = ''' + @action_user+''''            
 end            
            
 if @from_date is not null            
  and @to_date is not null            
 begin            
  set @sql += ' and ab.created_local_date between ''' + convert(varchar(10), @from_date, 101) + ''' and ''' + convert(varchar(10), dateadd(dd, 1, @to_date), 101) + ''''            
 end            
            
 set @sql += 'order by ab.created_local_date desc'            
 print @sql            
            
 exec (@sql)            
end --distributor balance report              
            
  if @flag = 'm'            
begin            
 begin try            
  begin transaction merchantbalancetransfer            
            
  if @agent_id is null            
  begin            
   select '1' code            
    ,'merchant id can''t be null' message            
    ,null id            
            
   return            
  end            
            
  if @amount < 0            
  begin            
   select '1' code            
    ,'amount can''t be less than ''0''' message            
    ,null id            
            
   return            
  end            
            
  insert into [dbo].tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,bank_id            
   ,bank_name            
   ,[amount]            
   ,txn_type            
   ,currency_code            
   ,agent_remarks            
   ,created_by            
   ,created_local_date            
   ,created_UTC_date            
   ,created_nepali_date            
   ,created_ip            
   )            
  values (            
   @agent_id            
   ,@agent_name            
   ,@bank_id            
   ,@bank_name            
   ,@amount            
   ,@type            
   ,'npr'            
   ,@remarks            
   ,@action_user            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
   ,@created_ip            
   )            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance, 0) + @amount            
  where agent_id = @agent_id            
            
  select '0' code            
   ,'successfully transfered balance of '+cast(@amount as varchar)+ ' to merchant: ' +@agent_name message            
   ,null id            
            
  --exec spa_email_request @flag='btd', @previous_balance=@distibutoroldbalance,@current_balance=@newdisbalance, @distributor_id=@distributer_id, @amount=@amount, @bankname=@bank_name                 
  --return                      
  commit transaction merchantbalancetransfer            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction merchantbalancetransfer            
            
  set @desc = 'sql error found:(' + error_message() + ')'            
            
  insert into tbl_error_log_sql(            
   sql_error_desc            
   ,sql_error_script            
   ,sql_query_string            
   ,sql_error_category            
   ,sql_error_source          
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(merchant balance transfer:flag ''m'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer(merchant balance transfer)'            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) message            
   ,null id            
 end catch            
end -- update balance for merchant by distibutor              
            
  if @flag = 'rm'            
begin            
 begin try            
  begin transaction merchantbalanceretrieve            
            
  select @agent_old_balance = isnull(available_balance, 0)            
  from tbl_agent_detail            
  where agent_id = @agent_id            
            
  if @amount < 0            
  begin            
   select '1' code            
    ,'amount can''t be less  than 0' message            
    ,null id            
            
   return            
  end            
            
  if @amount > isnull(@agent_old_balance, 0)            
  begin            
   select '1' code            
    ,'merchant balance is less than the amount for retrieval' message            
    ,null id            
            
   return            
  end            
            
  insert into tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,bank_id            
   ,bank_name            
   ,[amount]            
   ,txn_type            
   ,currency_code            
   ,agent_remarks            
   ,created_by            
   ,created_local_date            
   ,created_UTC_date            
   ,created_nepali_date            
   ,created_ip            
   )            
  values (            
   @agent_id            
   ,@agent_name            
   ,@bank_id            
   ,@bank_name            
   ,@amount            
   ,@type            
   ,'NPR'            
   ,@remarks            
   ,@action_user             
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
   ,@created_ip            
   )            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance, 0) - @amount            
  where agent_id = @agent_id            
            
  select '0' code            
   ,'successfully retrieved balance ' +cast(@amount as varchar)+' from merchant: '+@agent_name message            
   ,null id            
            
  --exec spa_email_request @flag='brd', @previous_balance=@distibutoroldbalance,@current_balance=@newdisbalance, @distributor_id=@distributer_id, @amount=@amount, @bankname=@bank_name                 
  --return                
  commit transaction merchantbalanceretrieve            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction merchantbalanceretrieve            
            
  set @desc = 'sql error found:(' + error_message() + ')'            
            
  insert into tbl_error_log_sql(            
   sql_error_desc            
   ,sql_error_script            
   ,sql_query_string            
   ,sql_error_category            
   ,sql_error_source            
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(merchant balance retrieve:flag ''rm'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer(merchant balance retrieve)'            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) message            
   ,null id            
 end catch            
end -- retrieve merchant balance by distributor              
            
  if @flag = 'st'            
begin            
 begin try            
  begin transaction subdistributorbalancetransfer            
            
  if @agent_id is null            
  begin            
   select '1' code            
    ,'sub distributor id can''t be null' message            
    ,null id            
            
   return            
  end            
            
  if @amount < 0            
  begin            
   select '1' code            
    ,'amount can''t be less than ''0''' message            
    ,null id            
            
   return            
  end            
            
  insert into [dbo].tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,bank_id            
   ,bank_name            
   ,[amount]            
   ,txn_type            
   ,currency_code            
   ,agent_remarks            
   ,created_by            
   ,created_local_date            
   ,created_UTC_date            
   ,created_nepali_date            
   ,created_ip            
   )            
  values (            
   @agent_id            
   ,@agent_name            
   ,@bank_id            
   ,@bank_name            
   ,@amount            
   ,@type            
   ,'npr'            
   ,@remarks            
   ,@action_user            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)           
   ,@created_ip            
   )            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance, 0) + @amount            
  where agent_id = @agent_id            
            
  select '0' code            
   ,'successfully transfered balance of '+cast(@amount as varchar)+' to sub-distributor: '+@agent_name message            
   ,null id            
            
  --exec spa_email_request @flag='btd', @previous_balance=@distibutoroldbalance,@current_balance=@newdisbalance, @distributor_id=@distributer_id, @amount=@amount, @bankname=@bank_name                 
  --return                      
  commit transaction subdistributorbalancetransfer            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction subdistributorbalancetransfer            
            
  set @desc = 'sql error found:(' + error_message() + ')'            
            
  insert into tbl_error_log_sql(            
   sql_error_desc            
   ,sql_error_script            
   ,sql_query_string            
   ,sql_error_category            
   ,sql_error_source            
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(sub distributor balance transfer:flag ''st'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer(sub distributor balance transfer)'            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) message            
   ,null id            
 end catch            
end -- update balance for sub distributor by distributor              
            
  if @flag = 'rs'            
begin            
 begin try            
  begin transaction subdistributorbalanceretrieve            
            
  select @agent_old_balance = isnull(available_balance, 0)            
  from tbl_agent_detail            
  where agent_id =  @agent_id            
            
  if @amount < 0            
  begin            
   select '1' code            
    ,'amount can''t be less  than 0' message            
    ,null id            
            
   return            
  end            
            
  if @amount > isnull(@agent_old_balance, 0)            
  begin            
   select '1' code            
    ,'sub distributor balance is less than the amount for retrieval' message            
    ,null id            
            
   return            
  end            
            
  insert into [dbo].tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,bank_id            
   ,bank_name            
   ,[amount]            
   ,txn_type            
   ,currency_code            
   ,agent_remarks            
,created_by            
   ,created_local_date            
   ,created_UTC_date            
   ,created_nepali_date            
   ,created_ip            
   )            
  values (            
   @agent_id            
   ,@agent_name            
   ,@bank_id            
   ,@bank_name            
   ,@amount            
   ,@type            
   ,'NPR'            
   ,@remarks            
   ,@action_user            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
   ,@created_ip            
   )            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance, 0) - @amount            
  where agent_id = @agent_id            
            
  select '0' code            
   ,'successfully retrieved balance of '+cast(@amount as varchar)+' from sub-distributor: '+@agent_name message            
   ,null id            
            
  --exec spa_email_request @flag='brd', @previous_balance=@distibutoroldbalance,@current_balance=@newdisbalance, @distributor_id=@distributer_id, @amount=@amount, @bankname=@bank_name                 
  --return                
  commit transaction subdistributorbalanceretrieve            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction subdistributorbalanceretrieve            
            
  set @desc = 'sql error found:(' + error_message() + ')'            
            
  insert into tbl_error_log_sql(            
   sql_error_desc         
   ,sql_error_script            
   ,sql_query_string            
   ,sql_error_category            
   ,sql_error_source            
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(sub distributor balance retrieve:flag ''rs'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer(sub distributor balance retrieve)'            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) message            
   ,null id            
 end catch            
end -- retrieve sub distributor balance                  
                      
  if @flag = 'at' -- agent Balance Transfer by admin            
  begin            
   begin try            
    begin transaction Agentbalancetransfer            
            
    if @agent_id is null            
    begin            
     select '1' code            
      ,'Agent id can''t be null' message            
      ,null id            
            
     return            
    end            
                
    if @amount < 0            
    begin            
     select '1' code            
      ,'Amount can''t be less than ''0''' message            
      ,null id            
            
     return            
    end            
            
    if @agent_parent_Id is not null            
    begin            
     if @amount > @agent_parent_balance            
     begin            
      select '1' Code, 'Parent Distributor Balance is less the Transfer Amount' Message, null id            
      return            
     end            
    end            
    else            
    begin            
                
     insert into [dbo].tbl_agent_balance(            
     agent_id            
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip            
     ,txn_mode       
     ,agent_parent_id  
     )            
    values (            
     @agent_id            
     ,@agent_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip            
     ,'CR'            
     ,@agent_parent_Id  
     )            
            
            
    update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) + @amount            
    where agent_id = @agent_id             
    --and Parent_id = @agent_parent_Id            
                
    end            
            
            
            
    select '0' code            
     ,'successfully transfered balance of ' + cast(@amount as varchar) +' to Agent: ' + @agent_name message            
     ,null id            
                
    commit transaction Agentbalancetransfer            
    end try            
            
    begin catch            
    if @@trancount > 0            
     rollback transaction Agentbalancetransfer            
            
    set @desc = 'sql error found:(' + error_message() + ')'         
            
    insert into tbl_error_log_sql(            
     sql_error_desc            
     ,sql_error_script            
     ,sql_query_string            
     ,sql_error_category            
     ,sql_error_source            
     ,sql_error_local_date            
     ,sql_error_UTC_date            
     ,sql_error_nepali_date            
     )            
    select @desc            
     ,'sproc_balance_transfer(Agent balance transfer:flag ''i'')'            
     ,'sql'            
     ,'sql'            
     ,'sproc_balance_transfer(Agent balance transfer)'            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
            
    select '1' code            
     ,'errorid: ' + cast(scope_identity() as varchar) message            
     ,null id            
   end catch            
  end              
  if @flag = 'ar' -- agent Balance Retrieve by admin            
  begin            
   begin try            
  begin transaction AgentbalanceRetrieve            
            
    if @agent_id is null            
    begin            
     select '1' code            
      ,'Agent id can''t be null' message            
      ,null id            
            
     return            
    end            
                
    if @amount < 0            
    begin            
     select '1' code            
      ,'Amount can''t be less than ''0''' message            
      ,null id            
            
     return            
    end            
            
                
     if @amount > @agent_old_balance            
     begin            
      select '1' Code, 'Agent Balance is less the Retieval Amount' Message, null id            
      return            
     end            
    else            
    begin            
     insert into [dbo].tbl_agent_balance(            
     agent_id            
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip            
     ,txn_mode            
     ,agent_parent_id            
     )            
    values (            
     @agent_id            
     ,@agent_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip            
     ,'DR'            
     ,@agent_parent_Id            
     )            
            
            
    update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) - @amount            
    where agent_id = @agent_id             
    --and Parent_id = @agent_parent_Id            
                
    update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) + @amount            
    where agent_id = @agent_parent_Id             
            
    end            
       
    select '0' code            
     ,'successfully Retrieved balance of ' + cast(@amount as varchar) +' from Agent: ' + @agent_name message            
     ,null id            
                
    commit transaction AgentbalanceRetrieve            
    end try            
            
    begin catch            
    if @@trancount > 0            
     rollback transaction AgentbalanceRetrieve            
            
    set @desc = 'sql error found:(' + error_message() + ')'            
            
    insert into tbl_error_log_sql(            
     sql_error_desc            
     ,sql_error_script            
     ,sql_query_string            
     ,sql_error_category            
     ,sql_error_source            
     ,sql_error_local_date            
     ,sql_error_UTC_date            
     ,sql_error_nepali_date            
     )            
    select @desc            
     ,'sproc_balance_transfer(Agent balance retrieve:flag ''i'')'            
     ,'sql'            
     ,'sql'            
     ,'sproc_balance_transfer(Agent balance retrieve)'            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
            
    select '1' code            
     ,'errorid: ' + cast(scope_identity() as varchar) message            
     ,null id            
   end catch            
  end            
            
  if @flag = 're'            
  begin            
            
   set @sql = ' select balance_id, ab.agent_id, ab.agent_name,            
   adt.full_name as Parent_Distributor,            
   amount,             
      CASE when ab.txn_type = UPPER(''T'')then            
      ad.available_balance - amount            
      else             
      ad.available_balance + amount            
      end Agent_old_balance, fb.funding_bank_branch, fb.funding_account_number,bank_name,            
   txn_type, agent_remarks, txn_mode,            
   Isnull(ad.available_balance,0)as New_Balance,            
   ab.created_by,             
   ab.created_local_Date as Created_date,            
   ab.created_nepali_Date             
   from tbl_agent_balance ab            
   join tbl_agent_detail ad on ad.agent_id  = ab.agent_id            
  left join tbl_agent_detail adt on adt.agent_id  =  ab.agent_parent_id            
   left join tbl_funding_bank_account fb on fb.funding_bank_id = ab.bank_id             
   where 1 = 1'            
            
   if @balance_id is not null            
   begin            
    set @sql += 'and balance_id= ' +Cast(@balance_id as varchar)            
   end            
   if @type is not null            
   begin            
    set @sql += ' and txn_type=' + @type            
   end            
               
   if @agent_id is not null            
   begin            
    set @sql += ' and ab.agent_id = ' + cast(@agent_id as varchar)            
   end            
            
   if @agent_parent_Id is not null            
   begin            
    set @sql += ' and adt.agent_parent_id= ' + cast(@agent_parent_Id as varchar)            
   end            
            
   if @bank_id is not null            
   begin            
    set @sql += ' and ab.bank_id = ' + cast(@bank_id as varchar)            
   end            
            
               
   if @action_user is not null            
   begin            
    set @sql += ' and ab.created_by = ''' + @action_user+''''            
   end            
            
   if @from_date is not null            
    and @to_date is not null            
   begin            
    set @sql += ' and ab.created_local_date between ''' + convert(varchar(10), @from_date, 101) + ''' and ''' + convert(varchar(10), dateadd(dd, 1, @to_date), 101) + ''''            
   end            
            
   set @sql += 'order by ab.created_local_date desc'            
   print @sql            
            
   exec (@sql)            
  end --Agent balance report            
    
  if @flag = 'trq'  
  begin  
 if not exists(select 'x' from tbl_user_detail where user_name = @action_user)  
 begin  
  Select '1' code, 'User not Found' message, null id  
  return  
 end  
 else  
 begin  
  select @sender_id = user_id from tbl_user_detail where user_name = @action_user  
 end  
 if not exists (select 'x' from tbl_user_detail where user_mobile_no = @subscriber or user_email = @subscriber)  
 begin  
  select '1' code, 'Requested User Not Found' message, null id  
  return  
 end  
 else  
 begin  
  Select @reciever_id = user_id, @agent_id = agent_id, @email_id = user_email from tbl_user_detail where user_mobile_no = @subscriber or user_email = @subscriber   
 end  
  
 insert into tbl_agent_notification(
	notification_subject, 
	notification_body, 
	notification_type, 
	notification_status, 
	notification_to, 
	agent_id,
	user_id,
	read_status, 
	created_by,
	created_local_date, 
	created_UTC_date, 
	created_nepali_date
	)  
 values(
	'Balance Transfer Request',
	@description, 
	'Balance_Transfer', 
	'Y',
	@agent_id,
	@agent_id,
	@sender_id,
	'n', 
	@action_user, 
	GETDATE(), 
	GETUTCDATE(), 
	dbo.func_get_nepali_date(default)
	)  
    
 exec sproc_email_request @flag = 'bt',@agent_id= @agent_id, @email_id = @email_id,@amount = @amount,@subscriber_no = @subscriber  
  
 select '0' code, 'Succesfully sent Balance Request to User: '+ @subscriber message, @sender_id id  
 return  
  end  -- user to user balance request  
    
  if @flag = 'trf'            
begin            
                  
  if not exists(Select 'x' from tbl_user_detail where user_name = @action_user)  
  begin  
  Select '1' code, 'User not found' message, null id  
  return  
  end  
  else  
  begin  
  select @sender_id = user_id,  
    @user_mobile_number =user_mobile_no,   
    @user_email = user_email,   
    @user_full_name = u.full_name,  
    @sender_agent_id =u.agent_id,  
     @agent_current_balance = Isnull(a.available_balance,0)            
    ,@grand_parent_id = ad.agent_id            
    ,@parent_id = a.parent_id            
    from tbl_user_detail u   
    Join tbl_agent_Detail a on a.agent_id  = u.agent_id  
    left Join tbl_agent_detail ad on ad.agent_id = a.parent_id  
    where user_name = @action_user            
  end  
  
  if not exists ( select * from tbl_user_detail where  user_mobile_no = @subscriber or user_email = @subscriber)  
  begin  
  select '1' code, 'Receiving User Not found' message, null id  
  return  
  end  
  else  
  begin  
  select @reciever_id = user_id,            
  @reciever_agent_id = agent_id,  
  @reciever_full_name = full_name,  
  @reciever_email = u.user_email  
  from tbl_user_detail u       
  where             
  user_mobile_no = @subscriber or user_email = @subscriber   
  end  
    
  if @amount < 0            
 begin            
  select '1' code            
   ,'Amount should be more than 0' message            
   ,null id            
            
  return            
 end   
 
 if @agent_current_balance < @amount            
 begin            
  select '1' code            
   ,'Sender''s balance is less the amount to be transfered' message            
   ,null id            
            
  return            
 end  
    
 set @remarks_agent = 'Balance transfered by ' + @user_full_name + ' of ' + cast(@amount as varchar) + ' NPR'            
                               
 begin try            
  begin transaction usertouserbalancetrf            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance,0) + @amount            
  where agent_id = @reciever_agent_id            
            
  update tbl_agent_detail            
  set available_balance = isnull(available_balance,0) - @amount            
  where agent_id = @sender_agent_id   
  
	  insert into tbl_agent_notification(  
		notification_subject, 
		notification_body, 
		notification_type, 
		notification_status, 
		notification_to, 
		agent_id,
		user_id, 
		read_status,
		created_by,
		created_local_date, 
		created_UTC_date, 
		created_nepali_date)  
	 values(
		'Balance Transfer',
		@remarks_agent, 
		'Balance_Transfer', 
		'Y',
		@reciever_agent_id,
		@reciever_agent_id,
		@reciever_id,
		'n',
		@action_user, 
		GETDATE(), 
		GETUTCDATE(), 
		dbo.func_get_nepali_date(default)
		)  --- notification for receiver (receiving user)

	--insert into tbl_agent_notification(
	--	notification_subject, 
	--	notification_body, 
	--	notification_type, 
	--	notification_status, 
	--	notification_to, 
	--	agent_id,
	--	user_id, 
	--	read_status,
	--	created_by,
	--	created_local_date, 
	--	created_UTC_date, 
	--	created_nepali_date)  
	-- values(
	--	'Balance Transfer',
	--	'Balance transfered to ' + @reciever_full_name + '(id: ' + cast(@reciever_id as varchar) + ') of ' + cast(@amount as varchar) + ' NPR' , 
	--	'Balance_Transfer', 
	--	'Y',
	--	@sender_agent_id,
	--	@sender_agent_id,
	--	@sender_id, 
	--	'n',
	--	@action_user, 
	--	GETDATE(), 
	--	GETUTCDATE(), 
	--	dbo.func_get_nepali_date(default)
	--	)   ---  notification for sender (Sending user)
  
            
  -- insert into transaction detail table for user transfred by              
  --insert into tbl_transaction(            
  -- product_label            
  -- ,txn_type_id            
  -- ,company_id            
  -- ,subscriber_no            
  -- ,grand_parent_id            
  -- ,parent_id            
  -- ,agent_id            
  -- ,amount            
  -- ,service_charge            
  -- ,bonus_amt            
  -- ,status            
  -- ,user_id            
  -- ,created_UTC_date            
  -- ,created_local_date            
  -- ,created_nepali_date            
  -- ,created_by            
  -- ,created_ip            
  -- ,created_platform            
  -- ,agent_remarks            
  -- )            
  --values (            
  -- 'balancetransfer'            
  -- ,8            
  -- ,5            
  -- ,@subscriber_no            
  -- ,@grand_parent_id            
  -- ,@parent_id            
  -- ,@sender_agent_id            
  -- ,@amount            
  -- ,isnull(@scharge, 0)            
  -- ,isnull(@bonus_amt, 0)            
  -- ,'success'            
  -- ,@action_user            
  -- ,getutcdate()            
  -- ,getdate()            
  -- ,[dbo].func_get_nepali_date(default)            
  -- ,@sender_id            
  -- ,@created_ip            
  -- ,@created_platform            
  -- ,@remarks_agent            
  -- )            
          
insert into tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,amount            
   ,currency_code            
   ,agent_parent_id            
   ,txn_type            
   ,created_by            
   ,created_UTC_date            
   ,created_local_date            
   ,created_nepali_date            
   ,created_ip            
   ,created_platform            
   ,user_id            
   ,agent_remarks            
   ,txn_id    
   ,txn_mode  
   )            
  values (            
   @sender_agent_id             
   ,@sender_name            
   ,@amount            
   ,'NPR'            
   ,@parent_id            
   ,'p2p'            
   ,@action_user            
   ,[dbo].func_get_nepali_date(default)            
   ,getdate()            
   ,getutcdate()            
   ,@created_ip            
   ,@created_platform            
   ,@sender_id            
   ,@remarks_agent            
   ,@txn_id   
   ,'DR'  
   )            

  --set @txn_id = scope_identity()            
            
  -- insert into agent balance table for user transfered to              
  
  insert into tbl_agent_balance(            
   agent_id            
   ,agent_name            
   ,amount            
   ,currency_code            
   ,agent_parent_id            
   ,txn_type            
   ,created_by            
   ,created_UTC_date            
   ,created_local_date            
   ,created_nepali_date            
   ,created_ip            
   ,created_platform            
   ,user_id            
   ,agent_remarks            
   ,txn_id    
   ,txn_mode  
   )            
  values (            
   @reciever_agent_id            
   ,@reciever_full_name            
   ,@amount            
   ,'NPR'            
   ,@parent_id            
   ,'p2p'            
   ,@action_user            
   ,[dbo].func_get_nepali_date(default)            
   ,getdate()            
   ,getutcdate()            
   ,@created_ip            
   ,@created_platform            
   ,@reciever_id            
   ,@remarks_agent            
   ,@txn_id   
   ,'DR'  
   )            
            
  commit transaction usertouserbalancetrf            
            
  select '0' code, @remarks_agent message, null id            
 end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction usertouserbalancetrf            
            
  set @desc = 'sql error found:(' + error_message() + ')' + ' at ' + error_line()            
            
  insert into tbl_error_log_sql(            
   sql_error_desc            
   ,sql_error_script            
   , sql_query_string            
   ,sql_error_category            
   ,sql_error_source            
   ,sql_error_local_date            
   ,sql_error_UTC_date            
   ,sql_error_nepali_date            
   )            
  select @desc            
   ,'sproc_balance_transfer(user to user balance trf:flag ''trf'')'            
   ,'sql'            
   ,'sql'            
   ,'sproc_balance_transfer((user to user balance trf)'            
   ,getdate()            
   ,getutcdate()            
   ,[dbo].func_get_nepali_date(default)            
            
  select '1' code            
   ,'errorid: ' + cast(scope_identity() as varchar) message            
   ,null id            
 end catch            
end -- user to user  balance trf              
  
  if @flag  = 'aw'  -- agent to wallet balance transfer  
  begin  
    begin try            
    begin transaction Agenttowalletbalancetransfer            
            
    if @action_user is null            
    begin            
     select '1' code            
      ,'Agent id can''t be null' message            
      ,null id            
            
     return            
    end  
 else  
 begin  
  select @sender_agent_id = u.agent_id, @sender_name = u.full_name, @sender_id = u.user_id from 
  tbl_user_detail u join  
  tbl_agent_detail ad on ad.agent_id  = u.agent_id where user_name = @action_user or user_mobile_no = @action_user or user_email = @action_user    
 end  
                
    if @amount < 0            
    begin            
     select '1' code            
      ,'Amount can''t be less than ''0''' message            
      ,null id            
            
     return            
    end            
       
  select @reciever_agent_id =ad.agent_id,  
   @reciever_full_name = ad.full_name,
   @reciever_id = u.user_id
   from tbl_user_detail u   
  join tbl_agent_detail ad  on ad.agent_id = u.agent_id  
  where user_name = @user_name or user_mobile_no = @user_name or user_email = @user_name  
                
                
     insert into [dbo].tbl_agent_balance(            
     agent_id            
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip            
     ,txn_mode       
     ,agent_parent_id  
     )            
    values (            
     @reciever_agent_id            
     ,@reciever_full_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip            
     ,'CR'            
     ,@agent_parent_Id  
     )            
        
     insert into [dbo].tbl_agent_balance(            
     agent_id            
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip            
     ,txn_mode       
     ,agent_parent_id  
     )            
    values (            
     @sender_agent_id            
     ,@sender_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip            
     ,'DR'            
     ,@agent_parent_Id  
     )  
  
  
  
        
   --deduct balance tranfer for agent  
   update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) - @amount            
    where agent_id = @sender_agent_id    
  
 --add balance to transfer user  
 update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) + @amount            
    where agent_id = @reciever_agent_id             
    --and Parent_id = @agent_parent_Id            
                
   	  insert into tbl_agent_notification(  
		notification_subject, 
		notification_body, 
		notification_type, 
		notification_status, 
		notification_to, 
		agent_id,
		user_id, 
		read_status,
		created_by,
		created_local_date, 
		created_UTC_date, 
		created_nepali_date)  
	 values(
		'Balance Transfer',
		'Balance transfered by ' + @sender_name + ' of ' + cast(@amount as varchar) + ' NPR', 
		'Balance_Transfer', 
		'Y',
		@reciever_agent_id,
		@reciever_agent_id,
		@reciever_id,
		'n',
		@action_user, 
		GETDATE(), 
		GETUTCDATE(), 
		dbo.func_get_nepali_date(default)
		)  --- notification for receiver (receiving user)

	--insert into tbl_agent_notification(
	--	notification_subject, 
	--	notification_body, 
	--	notification_type, 
	--	notification_status, 
	--	notification_to, 
	--	agent_id,
	--	user_id, 
	--	read_status,
	--	created_by,
	--	created_local_date, 
	--	created_UTC_date, 
	--	created_nepali_date)  
	-- values(
	--	'Balance Transfer',
	--	'Balance transfered to ' + @reciever_full_name + '(id: ' + cast(@reciever_id as varchar) + ') of ' + cast(@amount as varchar) + ' NPR' , 
	--	'Balance_Transfer', 
	--	'Y',
	--	@sender_agent_id,
	--	@sender_agent_id,
	--	@sender_id, 
	--	'n',
	--	@action_user, 
	--	GETDATE(), 
	--	GETUTCDATE(), 
	--	dbo.func_get_nepali_date(default)
	--	)   ---  notification for sender (Sending user)             
            
            
            
    select '0' code            
     ,'successfully transfered balance of ' + cast(@amount as varchar) +' to Agent: ' + @reciever_full_name message            
     ,null id            
                
    commit transaction Agenttowalletbalancetransfer            
    end try            
            
    begin catch            
    if @@trancount > 0            
     rollback transaction Agenttowalletbalancetransfer            
            
    set @desc = 'sql error found:(' + error_message() + ')'            
            
    insert into tbl_error_log_sql(            
     sql_error_desc            
     ,sql_error_script            
     ,sql_query_string            
     ,sql_error_category            
     ,sql_error_source            
     ,sql_error_local_date            
     ,sql_error_UTC_date            
     ,sql_error_nepali_date            
     )            
    select @desc            
     ,'sproc_balance_transfer(Agent to wallet user balance transfer:flag ''awu'')'            
     ,'sql'            
     ,'sql'            
     ,'sproc_balance_transfer(Agent to wallet user balance transfer)'            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
            
    select '1' code            
     ,'errorid: ' + cast(scope_identity() as varchar) message            
     ,null id            
   end catch            
  
  end  
  
  if @flag = 'awu' -- walletUser Balance Transfer by admin            
  begin            
   begin try            
    begin transaction Walletuserbalancetransfer            
            
    if @agent_id is null            
    begin            
     select '1' code            
      ,'Agent id can''t be null' message            
      ,null id            
            
     return            
    end            
                
    if @amount < 0            
    begin            
     select '1' code            
      ,'Amount can''t be less than ''0''' message            
      ,null id            
            
     return            
    end    
	
	select 
		@user_id=u.user_id,
		@agent_name = u.full_name
	from 
		tbl_agent_detail a 
	join tbl_user_detail u 
	on a.agent_id=u.agent_id 
	where a.agent_id = @agent_id
            
     
insert into [dbo].tbl_agent_balance(            
     agent_id 
	 ,user_id
     ,agent_name            
     ,bank_id            
     ,bank_name            
     ,[amount]            
     ,txn_type            
     ,currency_code            
     ,agent_remarks            
     ,created_by            
     ,created_local_date            
     ,created_UTC_date            
     ,created_nepali_date            
     ,created_ip            
     ,txn_mode       
     ,agent_parent_id  
     )            
    values (            
     @agent_id  
	 ,@user_id
     ,@agent_name            
     ,@bank_id            
     ,@bank_name            
     ,@amount            
     ,@type            
     ,'NPR'            
     ,@remarks            
     ,@action_user            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
     ,@created_ip            
     ,'CR'            
     ,@agent_parent_Id  
     )            
            
            
    update tbl_agent_detail            
    set available_balance = isnull(available_balance, 0) + @amount            
    where agent_id = @agent_id             
    --and Parent_id = @agent_parent_Id   
	
	insert into tbl_agent_notification(  
		notification_subject, 
		notification_body, 
		notification_type, 
		notification_status, 
		notification_to, 
		agent_id,
		user_id, 
		read_status,
		created_by,
		created_local_date, 
		created_UTC_date, 
		created_nepali_date)  
	 values(
		'Balance Transfer',
		'Balance transfered by YO! Pay Admin of ' + cast(@amount as varchar) + ' NPR', 
		'Balance_Transfer', 
		'Y',
		@agent_id,
		@agent_id,
		@user_id,
		'n',
		'admin', 
		GETDATE(), 
		GETUTCDATE(), 
		dbo.func_get_nepali_date(default)
		)  --- notification for receiver (receiving user)
                          
            
    select '0' code            
     ,'successfully transfered balance of ' + cast(@amount as varchar) +' to Agent: ' + @agent_name message            
     ,null id            
                
    commit transaction Walletuserbalancetransfer            
    end try            
            
    begin catch            
    if @@trancount > 0            
     rollback transaction Walletuserbalancetransfer            
            
    set @desc = 'sql error found:(' + error_message() + ')'            
            
    insert into tbl_error_log_sql(            
     sql_error_desc            
     ,sql_error_script            
     ,sql_query_string            
     ,sql_error_category            
     ,sql_error_source            
     ,sql_error_local_date            
     ,sql_error_UTC_date            
     ,sql_error_nepali_date            
     )            
    select @desc            
     ,'sproc_balance_transfer(Wallet User balance transfer:flag ''awu'')'            
     ,'sql'            
     ,'sql'            
     ,'sproc_balance_transfer(Wallet User balance transfer)'            
     ,getdate()            
     ,getutcdate()            
     ,[dbo].func_get_nepali_date(default)            
            
    select '1' code            
     ,'errorid: ' + cast(scope_identity() as varchar) message            
     ,null id            
   end catch            
  end   
  
  
  
  end try            
            
 begin catch            
  if @@trancount > 0            
   rollback transaction            
            
  select 1 code            
   ,error_message() message            
   ,null id            
 end catch            
    END;     
GO


