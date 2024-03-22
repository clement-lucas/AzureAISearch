DROP TABLE Categories;

CREATE TABLE Categories (    
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,    
    Name NVARCHAR(255) NOT NULL UNIQUE, -- Ensuring category names are not null and unique
	Description NVARCHAR(MAX)
);


IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'臨床試験計画')  
    INSERT INTO Categories (Name, Description) VALUES (N'臨床試験計画', 'Clinical Trial Protocol documents');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'参加者同意書')  
    INSERT INTO Categories (Name, Description) VALUES (N'参加者同意書', 'Participant Consent Forms');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'試験薬情報')  
    INSERT INTO Categories (Name, Description) VALUES (N'試験薬情報', 'Investigational Product Information');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'安全性情報')  
    INSERT INTO Categories (Name, Description) VALUES (N'安全性情報', 'Safety Information related to the clinical trial');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'研究データ')  
    INSERT INTO Categories (Name, Description) VALUES (N'研究データ', 'Research Data including patient data and trial results');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'監査報告書')  
    INSERT INTO Categories (Name, Description) VALUES (N'監査報告書', 'Audit Reports of the clinical trial process');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'規制文書')  
    INSERT INTO Categories (Name, Description) VALUES (N'規制文書', 'Regulatory Documents for compliance and approvals');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'倫理委員会資料')  
    INSERT INTO Categories (Name, Description) VALUES (N'倫理委員会資料', 'Documents related to Ethics Committee reviews and decisions');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'研究費用')  
    INSERT INTO Categories (Name, Description) VALUES (N'研究費用', 'Details on Research Funding and financial documents');  
  
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = N'トレーニング資料')  
    INSERT INTO Categories (Name, Description) VALUES (N'トレーニング資料', 'Training Materials for trial staff and participants');