DROP TABLE Users ;

CREATE TABLE Users (      
    UserId INT IDENTITY(1,1) PRIMARY KEY,      
    Name NVARCHAR(255) NOT NULL, -- Ensuring user names are not null    
    Email NVARCHAR(255) NOT NULL UNIQUE -- Ensuring email addresses are not null and unique    
);
 
INSERT INTO Users (Name, Email) VALUES  
(N'Manivelarasan Tamizharasan', 'mtamizharasa@microsoft.com'),
(N'佐藤 健', 'sato.ken@example.com'),  
(N'鈴木 一郎', 'suzuki.ichiro@example.com'),  
(N'高橋 恵子', 'takahashi.keiko@example.com'),  
(N'田中 美咲', 'tanaka.misaki@example.com'),  
(N'渡辺 貴史', 'watanabe.takashi@example.com'),  
(N'伊藤 美紀', 'ito.miki@example.com'),  
(N'山本 拓哉', 'yamamoto.takuya@example.com'),  
(N'中村 裕子', 'nakamura.yuko@example.com'),  
(N'小林 良太', 'kobayashi.ryota@example.com'),  
(N'加藤 悠真', 'kato.yuma@example.com');  
