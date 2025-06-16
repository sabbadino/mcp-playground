How to Run McpServerConsoleApp in vs code 

- Under the .vscode folder there is the mcp.json file with the registration McpServerConsoleApp as an mcp server via STDIO
- change the absolute path in mcp.json according to the local path where you cloned this repo
- start mcp server in vs code (there is a link on the entry of the "my-mcp-server-weather" in the mcp.json
-- Check the output console in vs code to see if all is ok 
-- make sure your GitHub copilot chat is set to AGENT MODE 
-- make sure the mcp server is listed as available (select tool under the chat input box)
-- to debug the McpServerConsoleApp go to vs studio 2022 , attach to the McpServerConsoleApp.exe process, put break pints in McpServerConsoleApp\WeatherMcpTool.cs\Get_Weather method 
- NOTE THAT if an mcp server os registered in the global vs code settings file, no roots will be available 
- the WeatherMcpTool in McpServerConsoleApp, reply to a fixed text to the question about the weather and also write a file 
on the vs code workspace to show how mcp roots work. 
