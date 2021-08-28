# Rexpavo BackupManager
## What exactly is this? 
This console application allows you to download the latest commits of any branches of your projects.
The whole thing is designed as a console application to run in a task scheduler (e.g. daily at 01:00 am).
## And how does this thing work?
All you need to get this thing running are GitHub projects and a JSON file - pretty stripped down stuff!

```json
{
	"Environment": {
		"SavePath": "<SAVE FOLDER>",
		"Token": "<PERSONAL ACCESS TOKEN>"
	},
	"Projects": [{
			"Name": "<NAME>",
			"Organization": "<ORG>",
			"Branches": [{
				"Name": "<NAME>"
			}]
		}

	]
}
```
This is what the JSON you need looks like and these are the options:

## Environment
These are basic settings, which are needed for successful execution
|Setting|What it does  |
|--|--|
| SavePath | This is the folder in which the backup directory should be created (don't forget to use double backslash in the path). |
Token|This is your personal access token that you can create in GitHub, please make sure it has enough permissions to access users, repositories and organizations

## Projects
The projects are specified as an array of objects, each object represents one project
|Setting|What it does  |
|--|--|
| Name |The name of the project from which you want to backup branches|
|Organization **(optional)**|This setting only needs to be specified if your project is within an organization and not directly under your user
|Branches|An array of the branches you want to backup
## Branches
Branches are defined in the same way as projects, as an array of objects, which is located directly as a setting in the project. 
|Setting|What it does  |
|--|--|
|Name|The name of the branch you want to backup  |


## Logging
Each backup operation is logged and written to a log file under the following folder: *"C:\Users\USER\AppData\Local\Temp\Rexpavo\BackupManager\Logs"*


