USE [WePayNepal]
GO

/****** Object:  StoredProcedure [dbo].[sproc_agent_cardMgmt]    Script Date: 6/1/2020 10:25:23 PM ******/
DROP PROCEDURE [dbo].[sproc_agent_cardMgmt]
GO

/****** Object:  StoredProcedure [dbo].[sproc_agent_cardMgmt]    Script Date: 6/1/2020 10:25:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  
    
          
-- =============================================          
-- Author:  <Author,,samir khadka>          
-- Create date: <14/05/2020,,>          
-- Description: <agent card management,,>          
-- =============================================          
CREATE                    PROCEDURE [dbo].[sproc_agent_cardMgmt]           
 @flag char(3) = null,          
 @agent_id int  = null,          
 @user_name varchar(50) =null,          
 @card_no varchar(20) =  null,          
 @card_uid nvarchar(max) = null,          
 @card_type varchar(50) = null,          
 @card_txn_type varchar(50) = null,          
 @is_active char(3) =  null,          
 @action_user varchar(50) = null,          
 @issue_date datetime  =  null,          
 @user_mobile_no varchar(10) =  null,          
 @user_email varchar(20) =  null,          
 @created_ip varchar(30) =  null,          
 @req_id int = null,          
 @req_status varchar(20)= null,          
 @user_id int = null  ,        
 @requested_amount decimal(18,2) =  null,  
 @card_id int = null,
 @transfer_to_mobile varchar(10) = null
AS          
BEGIN          
 -- SET NOCOUNT ON added to prevent extra result sets from          
 -- interfering with SELECT statements.          
 SET NOCOUNT ON;          
 Declare @sql varchar(max), @new_card_no varchar(100), @id int   ,@available_balance decimal(18,2), @available_card_balance decimal(18,2), @transfer_to_user_id int;
 Declare @transfer_to_agent_id int,@transfer_to_user_name varchar(250)
 -- check if user exists         
 if @user_id is not null        
 begin        
 if not exists(select 'x' from tbl_user_detail u  join tbl_agent_detail a on a.agent_id = u.agent_id where u.user_id = @user_id)          
 begin          
  Select '1' Code, 'User Not Found' message, null id          
  return          
 end      
 else    
 begin    
 select @available_balance = available_Balance from tbl_user_detail u  join tbl_agent_detail a on a.agent_id = u.agent_id where u.user_id = @user_id    
  if @available_balance < @requested_amount    
  begin    
   select '1' code, 'User Balance is less than the Card requested Amount' message, null id    
   return    
  end    
 end    
end     
    
         
  if @requested_amount < 0        
  begin        
 Set @requested_amount = @requested_amount * -1        
  end        
        
 set @new_card_no = '1000'+cast(convert(numeric(12,0),rand() * 899999999999) + 100000000000 as varchar)          
          
          
 if @flag = 's' --select all cards or by username          
 begin          
  select a.agent_id,     
  a.agent_name,     
  u.user_id, u.user_email,     
  u.user_mobile_no,    
  ac.card_id,  
  ac.card_no,     
  ac.card_type,    
  ac.card_issued_date,     
  ac.card_expiry_date,     
  ac.is_active,     
  u.full_name,    
  ac.amount as available_balance,
  ac.is_transfer,
  ac.transfer_to
  --a.available_balance     
  from tbl_user_detail u          
  join tbl_agent_detail a on a.agent_id = u.agent_id          
  join tbl_agent_card_management ac on ac.agent_id = a.agent_id          
  where u.user_id = @user_id   
  union
  select a.agent_id,     
  a.agent_name,     
  u.user_id, u.user_email,     
  u.user_mobile_no,    
  ac.card_id,  
  ac.card_no,     
  ac.card_type,    
  ac.card_issued_date,     
  ac.card_expiry_date,     
  ac.is_active,     
  u.full_name,    
  ac.amount as available_balance,
  ac.is_transfer,
  ac.transfer_to
  --a.available_balance     
  from tbl_user_detail u          
  join tbl_agent_detail a on a.agent_id = u.agent_id          
  join tbl_agent_card_management ac on ac.agent_id = a.agent_id          
  where ac.transfer_to = @user_id   
 end          
          
 --insert/add cards for agent/customer          
 if @flag = 'i'          
 begin          
            
  if exists(select 'x' from tbl_agent_card_management where card_no = @new_card_no)          
  begin          
   select '1'code, 'Card No already Exists' message, null id          
   return          
  end          
          
  Insert into tbl_agent_card_management (agent_id, user_id, user_name, card_no, card_type, card_issued_date, card_expiry_date, card_txn_type,is_active, created_by, created_local_date, created_utc_date, created_nepali_date)          
  values(@agent_id,@user_id, @user_name, @new_card_no, @card_type, GETDATE(),DATEADD(year, 4,GETDATE()),@card_txn_type,'y',@action_user, GETDATE(), GETUTCDATE(),dbo.func_get_nepali_date(default))          
          
  Select '0' Code, 'Card Issued Successfully' message, @user_id int          
  return          
 end          
          
 if @flag = 'u'          
 begin          
  if not exists(select 'x' from tbl_agent_card_management where card_no = @card_no)          
  begin          
   Select '1' code, 'Card Detail Not Found' message, null id          
   return          
  end          
          
  if exists(select 'x' from tbl_agent_card_management where card_no = @card_no and isnull(is_active,'n') = 'n')          
  begin          
   Select '1' Code, 'Card Deactivated, please activate the card first to continue' message , null id          
   return          
  end          
          
  update tbl_agent_card_management set card_type = Isnull(@card_type, card_type),          
            card_txn_type = Isnull(@card_txn_type, card_type)          
            where card_no = @card_no           
            and user_id = @user_id          
          
  Select '0' code, 'Card Details Succesfully activated' message, null id          
  return          
 end --update card details/ only card type, card txn type can be updated          
          
 if @flag = 'e' -- enable/disable card          
 begin          
  if not exists(select 'x' from tbl_agent_card_management where card_no = @card_no)          
  begin          
   Select '1' code, 'Card Detail Not Found' message, null id          
   return          
  end          
  else          
  begin          
   update tbl_agent_card_management set is_active =           
   Case when is_active  = 'y' then 'n'         
    when is_active  = 'n' then 'y' end           
    where card_no = @card_no and user_id = @user_id          
   select '0' code, 'Card Status changed succesfully' message, null id          
   return          
  end          
 end          
          
 if @flag = 'r' --request card          
 begin          
  if @user_id is null          
  begin          
   Select '1' code, 'User Id is required'message, null id          
   return          
  end          
  else          
  begin          
   select @user_email= user_email, @user_mobile_no= user_mobile_no,@agent_id= a.agent_id, @available_balance = available_balance from tbl_user_Detail u           
   join tbl_agent_detail a on a.agent_id = u.agent_id          
   where u.user_id = @user_id          
  end          
         
  if @available_balance < @requested_amount    
  begin    
  select '1' Code, 'Your available balance is less than the requested Balance' Message, null id    
  return    
  end    
    
  if @card_type is null        
  begin        
 select '1' code, 'Card Type is required' message , null id        
 return        
  end        
        
  insert into tbl_agent_card_request(user_name,user_mobile_no, user_email,request_status, created_local_date,created_UTC_Date,created_by,created_ip,Card_type,requested_amount )           
  values(@user_name, @user_mobile_no, @user_email,'Approved',GETDATE(), GETUTCDATE(),@action_user, @created_ip, @card_type, @requested_amount)     
    
  insert into tbl_agent_card_management          
   (user_id, user_name, agent_id, card_no,card_type,is_active,card_issued_date,card_expiry_date,created_by,created_local_date,created_utc_date,created_nepali_date,amount)          
   values(@user_id, @user_name, @agent_id, @new_card_no, @card_type, 'Y', GETDATE(), DATEADD(YEAR,4,GETDATE()),@action_user, GETDATE(), GETUTCDATE(),dbo.func_get_nepali_date(default),@requested_amount)          
         
  update tbl_agent_detail set available_balance = isnull(available_balance,0) - @requested_amount where agent_id = @agent_id    
          
  select '0' code, 'Card requested successfully' message, null id          
  return          
 end          
          
 if @flag = 'a' --approve/reject card          
 begin          
  if not exists(select 'x' from tbl_agent_card_request where req_id = @req_id)          
  begin          
   select '0' code, 'Requested Data Not found' message, null id          
   return          
  end          
  else if exists(select 'x' from tbl_agent_card_request where req_id = @req_id and trim(request_status) <> 'Pending')          
  begin          
   select '0' code, 'Card already Approved or Rejected' message, null id          
   return          
  end          
  else          
  begin          
   update tbl_agent_card_request set request_status = @req_status,    
           updated_local_date = getdate(),           
           updated_UTC_Date =GETUTCDATE(),          
           updated_by =@action_user,          
           updated_ip  =@Created_ip                      
           where req_id = @req_id          
         
  select @requested_amount  = requested_amount from tbl_agent_card_request where req_id = @req_id     
      
 if @req_status  = 'Approved'      
 begin      
   insert into tbl_agent_card_management          
   (user_id, user_name, agent_id, card_no,card_type,is_active,card_issued_date,card_expiry_date,created_by,created_local_date,created_utc_date,created_nepali_date,amount)          
   values(@user_id, @user_name, @agent_id, @new_card_no, @card_type, 'Y', GETDATE(), DATEADD(YEAR,4,GETDATE()),@action_user, GETDATE(), GETUTCDATE(),dbo.func_get_nepali_date(default),@requested_amount)          
         
  update tbl_agent_detail set available_balance = isnull(available_balance,0) - @requested_amount where agent_id = @agent_id    
    
    
   select '0' code, 'Card ' +@req_status+ ' succesfully.' message, null id          
   return          
    end      
      
 Select '1' code,  'Card ' +@req_status+ ' succesfully.' message, null id       
 return      
  end          
          
  end          
          
 if @flag = 'l' --get all requested card          
 begin          
  select  req_id,user_name, user_mobile_no, user_email,card_type, request_status, created_local_date,requested_amount from tbl_agent_card_request where request_status= 'Pending' order by 1 desc          
 end     
   
 if @flag = 'ad' --add balance in card  
  begin  
  if @card_id is null          
    begin          
     Select '1' code, 'Invalid card or Card not Found' message, null id          
     return          
    end   
  else   
   begin   
  
   select @available_card_balance = amount, @user_id = user_id, @agent_id = agent_id from tbl_agent_card_management where card_id = @card_id  
  
   select @available_balance = available_balance from tbl_user_Detail u           
      join tbl_agent_detail a   
      on a.agent_id = u.agent_id          
      where u.user_id = @user_id   
   end  
  
  if @requested_amount is null OR @requested_amount = 0  
   begin  
    Select '1' code, 'Requested Amount is invalid' message, null id          
    return          
   end  
  if @available_balance < @requested_amount   
   begin  
    select '1' code, 'Your available balance is less than the requested Balance' Message, null id  
    return  
   end  
  
  update tbl_agent_card_management  
   set updated_by = @action_user,  
    updated_utc_date = GETUTCDATE(),  
    updated_local_date = GETDATE(),  
    updated_nepali_date = dbo.func_get_nepali_date(GETDATE()),  
    Amount = (@available_card_balance + @requested_amount)  
   where card_id=@card_id  
  
  update tbl_agent_detail set available_balance = isnull(available_balance,0) - @requested_amount where agent_id = @agent_id    
  Select '0' code, 'Balance transfer to card successfully'message, null id   
  
  end  
  
  if @flag = 'rb' -- remove blance from card  
  Begin  
   if @card_id is null          
    begin          
     Select '1' code, 'Invalid card or Card not Found'message, null id          
     return          
    end   
  else   
   begin   
  
   select @available_card_balance = amount, @user_id = user_id, @agent_id = agent_id from tbl_agent_card_management where card_id = @card_id  
  
   select @available_balance = available_balance from tbl_user_Detail u           
      join tbl_agent_detail a   
      on a.agent_id = u.agent_id          
      where u.user_id = @user_id   
   end  
  
  if @requested_amount is null OR @requested_amount = 0  
   begin  
    Select '1' code, 'Requested Amount is invalid' message, null id          
    return          
   end  
  if @available_card_balance < @requested_amount   
   begin  
    select '1' code, 'The requested Balance is more than your card balance' Message, null id  
    return  
   end  
  
  update tbl_agent_card_management  
   set updated_by = @action_user,  
    updated_utc_date = GETUTCDATE(),  
    updated_local_date = GETDATE(),  
    updated_nepali_date = dbo.func_get_nepali_date(GETDATE()),  
    Amount = (@available_card_balance - @requested_amount)  
   where card_id=@card_id  
  
  update tbl_agent_detail set available_balance = isnull(available_balance,0) + @requested_amount where agent_id = @agent_id    
  
    Select '0' code, 'Balance transfer to wallet successfully'message, null id         
  
  End  

  if @flag ='tr' --transfer card authority
  Begin
  if @card_id is null          
    begin          
     Select '1' code, 'Invalid card or Card not Found' message, null id          
     return          
    end  
  if @transfer_to_mobile is null          
    begin          
     Select '1' code, 'Invalid User Info' message, null id          
     return          
    end
  if @transfer_to_mobile is not null               
	 if not exists(select 'x' from tbl_user_detail u  join tbl_agent_detail a on a.agent_id = u.agent_id where u.user_mobile_no = @transfer_to_mobile)          
	 begin          
	  Select '1' Code, 'User Not Found' message, null id          
	  return          
	 end      
  else
  begin
  select @transfer_to_user_id = user_id, @transfer_to_agent_id=u.agent_id,@transfer_to_user_name=user_name from tbl_user_Detail u           
      join tbl_agent_detail a   
      on a.agent_id = u.agent_id          
      where u.user_mobile_no = @transfer_to_mobile
  
  end

	update tbl_agent_card_management  
	   set updated_by = @action_user,  
		updated_utc_date = GETUTCDATE(),  
		updated_local_date = GETDATE(),  
		updated_nepali_date = dbo.func_get_nepali_date(GETDATE()),  
		is_transfer =  'Y',
		transfer_to = @transfer_to_user_id
	   where card_id= @card_id
	Select '0' code, 'Card transfer successfully to '+@transfer_to_mobile message, null id  

  End
  
  if @flag ='re' --retrieve card authority
  Begin
  if @card_id is null          
    begin          
     Select '1' code, 'Invalid card or Card not Found' message, null id          
     return          
    end  

	update tbl_agent_card_management  
	   set updated_by = @action_user,  
		updated_utc_date = GETUTCDATE(),  
		updated_local_date = GETDATE(),  
		updated_nepali_date = dbo.func_get_nepali_date(GETDATE()),  
		is_transfer = NULL,
		transfer_to = NULL
	   where card_id= @card_id
	Select '0' code, 'Card retrieve successfully ' message, null id  

  End
  end          
GO


