DROP TABLE Information;

CREATE TABLE Information (    
    InformationId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL UNIQUE, -- Ensuring Information names are not null and unique
    Description NVARCHAR(MAX)   
);

INSERT INTO [dbo].[Information] (Name, Description) VALUES  
(N'治験概要', 'Provides a high-level summary of the clinical trial, including objectives, design, and expected outcomes.'),  
(N'被験者基準', 'Details the inclusion and exclusion criteria for trial participants.'),  
(N'治験薬情報', 'Contains information about the investigational product(s) being tested, including composition, dosing, and administration.'),  
(N'安全性監視計画', 'Outlines the procedures for monitoring and reporting adverse events and other safety information.'),  
(N'データ管理計画', 'Describes the processes for data collection, entry, verification, and analysis.'),  
(N'統計分析計画', 'Provides detailed information on statistical methods and analyses to be used in evaluating the trial data.'),  
(N'研究成果', 'Summarizes the results and findings of the clinical trial.'),  
(N'監査報告', 'Contains findings and recommendations from audits conducted to ensure compliance with study protocols and regulations.'),  
(N'エンドポイントデータ', 'Detailed data on primary and secondary endpoints assessed during the trial.'),  
(N'合意形成書', 'Copies of the informed consent forms signed by participants, detailing the trial''s purpose, procedures, risks, and benefits.');  
