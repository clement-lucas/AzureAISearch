DROP TABLE Protocols;

CREATE TABLE Protocols (  
    ProtocolId INT IDENTITY(1,1) PRIMARY KEY,  
    ProtocolName NVARCHAR(255) NOT NULL,  
    Description NVARCHAR(MAX)  
);  

INSERT INTO Protocols (Name, Description) VALUES  
(N'治験プロトコルA', N'このプロトコルは、疾患Xに対する新薬Yの有効性を評価するためのものです。'),  
(N'治験プロトコルB', N'このプロトコルは、治療法Zの安全性プロファイルを詳細に調査するための多施設共同研究です。'),  
(N'治験プロトコルC', N'この研究は、特定の患者集団における介入Aの長期的な影響を調査することを目的としています。'),  
(N'プロトコルDの評価', N'プロトコルDは、既存の治療法と新しい介入法を比較するためのランダム化比較試験です。'),  
(N'プロトコルEの実施', N'このプロトコルは、特定の医薬品の用量応答関係を明らかにするための初期フェーズの研究です。');  

 
