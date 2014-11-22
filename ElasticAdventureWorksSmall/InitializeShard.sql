/*
    Copyright 2014 Microsoft, Corp.

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

-- Reference table that contains the same data on all shards
IF OBJECT_ID('Regions', 'U') IS NULL 
BEGIN
    CREATE TABLE [Regions] (
        [RegionId] [int] NOT NULL,
        [Name] [nvarchar](256) NOT NULL
     CONSTRAINT [PK_Regions_RegionId] PRIMARY KEY CLUSTERED (
        [RegionId] ASC
     ) 
    ) 

	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (0, 'North America')
	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (1, 'South America')
	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (2, 'Europe')
	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (3, 'Asia')
	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (4, 'Africa')
	INSERT INTO [Regions] ([RegionId], [Name]) VALUES (5, 'Oceania')
END
GO

CREATE SCHEMA HumanResources
GO

-- Sharded table containing our sharding key (CustomerId)
IF OBJECT_ID('[HumanResources].[vEmployee]', 'U') IS NULL 


	CREATE TABLE [HumanResources].[vEmployee](
		[BusinessEntityID] [int] NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
		[RegionId] [int] NOT NULL
	 CONSTRAINT [PK_vEmployee] PRIMARY KEY CLUSTERED 
	(
		[BusinessEntityID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	)

GO
