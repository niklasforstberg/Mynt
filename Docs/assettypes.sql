INSERT INTO [Mynt].[dbo].[AssetTypes] ([DefaultName], [IsAsset], [IsPhysical], [CreatedAt], [UpdatedAt])
VALUES 
    ('Equities', 1, 0, GETDATE(), GETDATE()),
    ('Bonds', 1, 0, GETDATE(), GETDATE()),
    ('Cash', 1, 0, GETDATE(), GETDATE()),
    ('Real Estate', 1, 1, GETDATE(), GETDATE()),
    ('Gold', 1, 1, GETDATE(), GETDATE()),
    ('Silver', 1, 1, GETDATE(), GETDATE()),
    ('Oil', 1, 0, GETDATE(), GETDATE()),
    ('Natural Gas', 1, 0, GETDATE(), GETDATE()),
    ('Cryptocurrency', 1, 0, GETDATE(), GETDATE()),
    ('Private Equity', 1, 0, GETDATE(), GETDATE()),
    ('Hedge Funds', 1, 0, GETDATE(), GETDATE()),
    ('Art', 1, 1, GETDATE(), GETDATE()),
    ('Collectibles', 1, 1, GETDATE(), GETDATE()),
    ('Wine', 1, 1, GETDATE(), GETDATE()),
    ('ETFs', 1, 0, GETDATE(), GETDATE()),
    ('Mutual Funds', 1, 0, GETDATE(), GETDATE()),
    ('Options', 1, 0, GETDATE(), GETDATE()),
    ('Futures', 1, 0, GETDATE(), GETDATE()),
    ('Commodities', 1, 0, GETDATE(), GETDATE());