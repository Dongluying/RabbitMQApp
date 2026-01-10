# RabbitMQApp

1. In this project, a RabbitMQ is created in docker
2. Then create a console sender application to send Order object to RabbitMQ
3. Create a console application hosting the worker service to receive the order object from RabbitMQ
4. Add a Serilog logger to both sender and receiver project. write log to a text file. The logger is configured in appsettings.json file 
