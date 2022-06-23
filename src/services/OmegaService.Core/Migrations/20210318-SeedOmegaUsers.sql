-- Insert rows into table 'OmegaUser' in schema '[dbo]'
INSERT INTO [dbo].[OmegaUser]
( -- Columns to insert data into
    [FirstName], [LastName], [Email]
)
VALUES
( -- First row: values for the columns in the list above
    'Chuck', 'Norris', 'chuck.norris@test.com'
),
( -- Second row: values for the columns in the list above
    'Your', 'Mom', 'your.mom@test.com'
)
-- Add more rows here
GO
