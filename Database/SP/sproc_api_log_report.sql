USE [WePayNepal]
GO
/****** Object:  StoredProcedure [dbo].[sproc_api_log_report]    Script Date: 6/2/2020 7:27:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   PROCEDURE [dbo].[sproc_api_log_report]
@flag varchar (10) = null,
@api_log_id varchar (20) = null,
 @from_date varchar(20) = null,
@to_date varchar(20) = null	
AS
Declare @sql varchar(max)
BEGIN
If @flag = 's' 
begin
SET @sql = 'select * 
from tbl_api_log al
left join tbl_authorization_request ar
on al.user_id = ar.authorization_token
left join tbl_user_detail ud 
on  ar.request_user = ud.user_id where 1=1'


	  IF (@api_log_id IS NOT NULL)  
	   BEGIN  
		SET @sql = @sql + ' and al.api_log_id = ''' + @api_log_id + '''';  
	   END; 

	    if(@from_date is not null and @to_date is not null)
	begin
	set @sql = @sql + ' and al.created_UTC_date between '''+ @from_date + ''' and '''+ @to_date +''''; 
		end	;	


	    PRINT @sql;  
  
	   EXEC (@sql);

end
END


