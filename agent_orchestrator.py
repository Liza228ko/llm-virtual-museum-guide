import google.generativeai as genai
import json
import os
import asyncio
import copy
import logging
from datetime import datetime
from agent_prompts import (
    INIT_PROMPT,
    get_classify_prompt, 
    get_navigate_prompt, 
    get_info_prompt, 
    get_preference_prompt
)

logger = logging.getLogger(__name__)

# WARNING: Do not commit this file with the key pasted in. This is for local testing only.
api_key = os.getenv("GEMINI_API_KEY") # <-- Set this in your .env file

if not api_key or api_key == "PASTE_YOUR_GEMINI_API_KEY_HERE":
    raise ValueError("API key not found. Please paste your Gemini API key directly into the bots.py file.")
genai.configure(api_key=api_key)

# Load museum data from JSON file
with open('museum_data.json', 'r', encoding='utf-8') as f:
    museum_data = json.load(f)
    MUSEUM_EXHIBITS = json.dumps(museum_data, indent=2)

# Create the model with system instruction
SYSTEM_INSTRUCTION = f"""{INIT_PROMPT}

The museum's exhibits are:
{MUSEUM_EXHIBITS}
"""

model = genai.GenerativeModel(
    'models/gemini-2.5-flash-lite',
)

GLOBAL_CHAT_HISTORY = [
    {
        "role": "model",
        "parts": [{"text": SYSTEM_INSTRUCTION}]
    }
]


async def classify(query: str, current_object: str, current_position: str, visited_objects: str) -> str:
    """Classify the user's query intent."""
    prompt = get_classify_prompt(query, current_object, current_position, visited_objects, MUSEUM_EXHIBITS)
    input_prompt = prompt + "INPUT:" + query + "RESPONSE:"
    response = await model.generate_content_async(input_prompt)
    return response.text.strip()


async def information_bot(query: str, current_object: str, current_position: str, visited_objects: str) -> dict:
    """Handle information requests about exhibits."""
    info_chat_history = copy.deepcopy(GLOBAL_CHAT_HISTORY)
    prompt = get_info_prompt(query, current_object, current_position, visited_objects, MUSEUM_EXHIBITS)
    
    # FIX: Build context with current exhibit info
    input_prompt = f"\nUSER QUERY: {query}"
    if current_object and current_object.strip():
        input_prompt = f"\nCURRENT EXHIBIT: I am currently looking at '{current_object}'.\nUSER QUERY: {query}"
    
    if visited_objects and len(visited_objects) > 2:
        input_prompt += f"\nVISITED EXHIBITS: {visited_objects}"

    print(f"[INFO BOT] Current Object: {current_object}")
    print(f"[INFO BOT] Visited: {visited_objects}")
   
    info_chat_history[-1]["parts"][0]["text"] = prompt + input_prompt + "\nRESPONSE:"
    response = await model.generate_content_async(info_chat_history)
    
    try:
        text_response = response.text.strip()
        # Remove markdown code blocks if present
        if text_response.startswith("```json"):
            text_response = text_response[7:]
        if text_response.endswith("```"):
            text_response = text_response[:-3]
        text_response = text_response.strip()
        return json.loads(text_response)
    except (json.JSONDecodeError, AttributeError) as e:
        print(f"[INFO BOT] Error decoding response: {e}")
        print(f"[INFO BOT] Raw response: {response.text}")
        return {"error": "Failed to decode the response from the language model.", "raw": response.text}


async def navigate_bot(query: str, current_object: str, current_position: str, visited_objects: str) -> dict:
    """Handle navigation requests to different exhibits."""
    nav_chat_history = copy.deepcopy(GLOBAL_CHAT_HISTORY)
    prompt = get_navigate_prompt(query, current_object, current_position, visited_objects, MUSEUM_EXHIBITS)
    
    # FIX: Build proper context
    input_prompt = f"\nUSER QUERY: {query}"
    if current_object and current_object.strip():
        input_prompt = f"\nCURRENT EXHIBIT: {current_object}\nUSER QUERY: {query}"
    
    if visited_objects and len(visited_objects) > 2:
        input_prompt += f"\nVISITED EXHIBITS: {visited_objects}"
    
    print(f"[NAVIGATE BOT] Current Object: {current_object}")
    print(f"[NAVIGATE BOT] Visited: {visited_objects}")
   
    nav_chat_history[-1]["parts"][0]["text"] = prompt + input_prompt + "\nRESPONSE:"
    response = await model.generate_content_async(nav_chat_history)
    
    try:
        text_response = response.text.strip()
        # Remove markdown code blocks if present
        if text_response.startswith("```json"):
            text_response = text_response[7:]
        if text_response.endswith("```"):
            text_response = text_response[:-3]
        text_response = text_response.strip()
        
        result = json.loads(text_response)
        
        # FIX: Log the tour being suggested
        if "tour_ids" in result:
            print(f"[NAVIGATE BOT] Suggesting tour: {result['tour_ids']}")
        
        return result
    except (json.JSONDecodeError, AttributeError) as e:
        print(f"[NAVIGATE BOT] Error decoding response: {e}")
        print(f"[NAVIGATE BOT] Raw response: {response.text}")
        return {"error": "Failed to decode the response from the language model.", "raw": response.text}


async def preference_bot(query: str, current_object: str, current_position: str, visited_objects: str) -> dict:
    """Handle user preference specifications."""
    pref_chat_history = copy.deepcopy(GLOBAL_CHAT_HISTORY)
    prompt = get_preference_prompt(query, current_object, current_position, visited_objects, MUSEUM_EXHIBITS)
    pref_chat_history[-1]["parts"][0]["text"] = prompt
    response = await model.generate_content_async(pref_chat_history)
    
    try:
        text_response = response.text.strip()
        # Remove markdown code blocks if present
        if text_response.startswith("```json"):
            text_response = text_response[7:]
        if text_response.endswith("```"):
            text_response = text_response[:-3]
        text_response = text_response.strip()
        return json.loads(text_response)
    except (json.JSONDecodeError, AttributeError) as e:
        print(f"[PREFERENCE BOT] Error decoding response: {e}")
        print(f"[PREFERENCE BOT] Raw response: {response.text}")
        return {"response": "Thank you for sharing that information. I'll keep that in mind as we explore the museum!"}
    
async def error_bot(query: str) -> dict:
    response = await model.generate_content_async(GLOBAL_CHAT_HISTORY)
    try:
        text_response = response.text.strip()
        # Remove markdown code blocks if present
        if text_response.startswith("```json"):
            text_response = text_response[7:]
        if text_response.endswith("```"):
            text_response = text_response[:-3]
        text_response = text_response.strip()
        return json.loads(text_response)
    except (json.JSONDecodeError, AttributeError) as e:
        print(f"[Error BOT] Error decoding response: {e}")
        print(f"[Error BOT] Raw response: {response.text}")
        return {"response": "Non related information!"}


async def orchestrate_task(query: str, current_object: str = "", current_position: str = "", visited_objects: str = "") -> dict:
    """
    Orchestrates the task by classifying the user's intent and calling the appropriate bot.
    """
    
    # FIX: Log the current state
    print(f"\n[ORCHESTRATE] Query: {query}")
    print(f"[ORCHESTRATE] Current Object: {current_object}")
    print(f"[ORCHESTRATE] Visited Objects: {visited_objects}")

    # Add the user's query
    GLOBAL_CHAT_HISTORY.append({
        "role": "user",
        "parts": [{"text": query}]
    })

    # Step 1: Classify the user's intent
    task = await classify(query, current_object, current_position, visited_objects)
    print(f"[ORCHESTRATE] Classification result: {task}")
    
    # Step 2: Call the appropriate bot based on the classification
    if "navigation" in task.lower():
        result = await navigate_bot(query, current_object, current_position, visited_objects)
    elif "information" in task.lower():
        result = await information_bot(query, current_object, current_position, visited_objects)
    elif "preference" in task.lower():
        result = await preference_bot(query, current_object, current_position, visited_objects)
    elif "error" in task.lower():
        result = await error_bot(query)
    else:
        # Fallback to information
        print("[ORCHESTRATE] Unknown classification, falling back to information bot")
        result = await information_bot(query, current_object, current_position, visited_objects)

    model_response_text = json.dumps(result) if isinstance(result, dict) else str(result)

    GLOBAL_CHAT_HISTORY.append({
        "role": "model",
        "parts": [{"text": model_response_text}]
    })

    return [result, task.lower()]