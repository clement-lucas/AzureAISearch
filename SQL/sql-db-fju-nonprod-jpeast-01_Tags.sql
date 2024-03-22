DROP TABLE Tags;

CREATE TABLE Tags (  
    TagId INT IDENTITY(1,1) PRIMARY KEY,  
    Name NVARCHAR(255) NOT NULL  
); 

INSERT INTO Tags (TagName) VALUES  
(N'緊急'),  
(N'重要'),  
(N'長期追跡'),  
(N'安全性'),  
(N'有効性'),  
(N'ランダム化'),  
(N'二重盲検'),  
(N'プラセボ対照'),  
(N'多施設'),  
(N'国際共同'),  
(N'介入研究'),  
(N'観察研究'),  
(N'予防'),  
(N'治療'),  
(N'診断'),  
(N'スクリーニング'),  
(N'品質改善'),  
(N'経済評価');  

 
