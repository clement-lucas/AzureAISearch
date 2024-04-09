DROP TABLE Documents;
  
CREATE TABLE Documents (      
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,  
    FileName NVARCHAR(255) NOT NULL,  
    Description NVARCHAR(MAX),  
	Version NVARCHAR(50),  
    BlobStorageUri NVARCHAR(MAX), -- Storing the URI to the Blob Storage file  
    CategoryId INT,  
    ProtocolId INT,  
	CreatedById INT,  
    ModifiedById INT,  
	DisclosureScopeId INT,    
    InformationId INT,      
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),      
    FOREIGN KEY (ProtocolId) REFERENCES Protocols(ProtocolId),  
	FOREIGN KEY (CreatedById) REFERENCES Users(UserId),  
    FOREIGN KEY (ModifiedById) REFERENCES Users(UserId),  
    FOREIGN KEY (DisclosureScopeId) REFERENCES DisclosureScopes(DisclosureScopeId),  
	FOREIGN KEY (InformationId) REFERENCES Information(InformationId)  
);  