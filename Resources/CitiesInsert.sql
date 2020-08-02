USE [DB_A437B6_stefanpeev]
GO

INSERT INTO [dbo].[Cities]
           ([CountryId]
           ,[CityName]
           ,[PostalCode]
           ,[PhoneCode]
           ,[CityCode]
           ,[Latitude]
           ,[Longitude])
     VALUES
           (<CountryId, int,>
           ,<CityName, nvarchar(85),>
           ,<PostalCode, nvarchar(10),>
           ,<PhoneCode, nvarchar(50),>
           ,<CityCode, nvarchar(20),>
           ,<Latitude, nvarchar(max),>
           ,<Longitude, nvarchar(max),>)
GO

