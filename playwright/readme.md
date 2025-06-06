- make sure @playwright/test is installed 
- go to copilot chat , agent mode 
- select file 'prompt1.md' so that copilot agent is aware of it 
- type in agent chat 'run the prompt'

add in your .vscode\mcp.json file this mcp server
<pre>
  "playwright-executeautomation": {
            "type": "stdio",
            "command": "npx",
            "args": [
                "@executeautomation/playwright-mcp-server@latest",
                 "--isolated"
            ]
        }
</pre>

P.S. : you can also try  MS provided playwright MCP server
<pre>
"playwright-ms": {
            "type": "stdio",
            "command": "npx",
            "args": [
                "@playwright/mcp@latest",
                 "--isolated"
            ]
        },        
</pre>

but i prefer the previous one since it offers the start / end session tool that generates the playwright actions executed: in this way I can instruct github copilot to start from this file to add the required test checks 
