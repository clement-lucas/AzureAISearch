IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_DocumentsDetails]'))  
    DROP VIEW [dbo].[vw_DocumentsDetails];  
GO  
  
CREATE VIEW [dbo].[vw_DocumentsDetails]  
AS  
SELECT     
    d.DocumentId,    
    d.FileName,    
    d.Description,    
    d.Version,    
    d.metadata_storage_path, -- Updated to include metadata_storage_path instead of BlobStorageUri  
    c.Name AS CategoryName,    
    p.Name AS ProtocolName,    
    u1.Name AS CreatedByName, -- Alias u1 for the user who created the document    
    u2.Name AS ModifiedByName, -- Alias u2 for the user who modified the document    
    ds.Name AS DisclosureScopeName,    
    i.Name AS InformationName    
FROM Documents d    
LEFT JOIN Categories c ON d.CategoryId = c.CategoryId    
LEFT JOIN Protocols p ON d.ProtocolId = p.ProtocolId    
LEFT JOIN Users u1 ON d.CreatedById = u1.UserId -- Joining Users table as u1 for CreatedById    
LEFT JOIN Users u2 ON d.ModifiedById = u2.UserId -- Joining Users table as u2 for ModifiedById    
LEFT JOIN DisclosureScopes ds ON d.DisclosureScopeId = ds.DisclosureScopeId    
LEFT JOIN Information i ON d.InformationId = i.InformationId;  
GO  
