from fastapi import FastAPI, Form
from agent_orchestrator import orchestrate_task
import json

app = FastAPI()

@app.get("/")
def read_root():
    return {"Hello": "World"}

@app.post("/ask")
async def ask(
    query: str = Form(...),
    currentObject: str = Form(""),
    currentAvatarPosition: str = Form(""),
    visitedObjects: str = Form("")
):
    [response, task] = await orchestrate_task(query, currentObject, currentAvatarPosition, visitedObjects)
    
    # Convert response dict to JSON string if it's a dict
    if isinstance(response, dict):
        content_string = json.dumps(response)
    else:
        content_string = str(response)

    print(f"[MAIN] Content string type: {type(content_string)}")
    
    # Return properly structured response
    return {
        "content": content_string,  # Now it's always a string
        "tasks": [task]
    }