# JobScheduler

The project is a basic console based task scheduler. If you want to create a new task type. You should do so by implementing the ITask interface. you should also assign a unique TaskType string to each new task created. Then you can create a Job with specific parameters and assign the task to it. Persistence is done by a simple JSON file. which sits in the same path as the executable. 

 Use the command 'get_state' to display the contents of the jobs.json file on the console.
