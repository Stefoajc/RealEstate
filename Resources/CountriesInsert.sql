USE [DB_A437B6_stefanpeev]
GO

INSERT INTO [dbo].[Countries]
           ([CountryNameBG]
           ,[CountryNameEN]
           ,[IsoCode]
           ,[CountryPhoneCode])
     VALUES
           (<CountryNameBG, nvarchar(max),>
           ,<CountryNameEN, nvarchar(max),>
           ,<IsoCode, nvarchar(3),>
           ,<CountryPhoneCode, int,>)
GO

