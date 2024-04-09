DROP TABLE DocumentTags;
  
CREATE TABLE DocumentTags (      
    DocumentId INT,      
    TagId INT,      
    PRIMARY KEY (DocumentId, TagId),      
    FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId),      
    FOREIGN KEY (TagId) REFERENCES Tags(TagId)      
);