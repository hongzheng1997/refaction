Hong Zheng

when I clone this to my local, it fails to get products. I found missing folder debug in refactor-me/bin/. not sure why. Once I add debug folder, it works fine.

Datalayer:

1. Create a Repository library project, copy Database.mdf and its log file from app_data into Repository folder.
2. Add ProductModel.edmx to create Product and ProductOption data model by using EF database to class approach with the db in Repository folder.
3. Create ProductData.cs to interact with database entites via EF auto generated database entites type and db context.
4. Implement all methods specified in the requirements and make them ready to be called from product controller


Refactor-me:

1. Create ProductsController constructor and inject in ProductData instance as private readonly object to be used in each controller method.
2. Replace original data model and methods with newly created db library model and methods to interact with database.
3. Add database connection string in web.config and pass in as parameter when injecting ProductData instance.
4. Exclude original files in models folder.
5. Use Async await methods.


Product Api tests:

create unit test project to test each method in Products controller is working as expected.


