ALTER TABLE AspNetUsers
ADD CONSTRAINT DF_customers_customerImage DEFAULT 'User-avatar.png' FOR customerImage;
