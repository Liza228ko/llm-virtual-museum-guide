# A Multi-Agent AI Approach to Virtual Museum Exploration

It is an interactive, 3D virtual museum showcasing the history of computing. It features an intelligent, embodied avatar that acts as a dynamic tour guide, leading users through exhibits using natural language interaction.

## Overview

This project bridges a **Unity 3D frontend** with the **Google Gemini API**, utilizing a **FastAPI proxy server** to manage communication. It addresses the limitations of traditional virtual tours (like limited interactivity and "one-size-fits-all" paths) by offering a context-aware, personalized, and multi-modal experience.

The core logic is driven by a **Multi-Agent Orchestration** system consisting of:
- **Classifier Agent:** Determines the user's intent from their query.
- **Navigation Agent:** Plans non-repetitive routes through the 3D environment based on user requests.
- **Information Agent:** Retrieves detailed, context-aware information about exhibits from a JSON knowledge base.
- **Preference Agent:** Captures user background (e.g., student vs. professor) to tailor the language and focus of the tour.

The guide can:
- **Understand user queries** regarding the museum or specific artifacts via text or voice.
- **Navigate an avatar** dynamically through the Unity NavMesh to specific exhibit locations.
- **Provide historical context** and answer follow-up questions while remembering conversation history and previously visited locations.

## 📺 Demo

Check out the project in action: [Demo Video](https://youtu.be/q9cnR1ttfE4)

## Tech Stack

- **Frontend:** Unity 3D, C#, Meta Voice SDK (TTS/STT).
- **Backend:** Python, FastAPI, Uvicorn.
- **AI/LLM:** Google Gemini 2.5 Flash-Lite (Optimized for real-time latency of ~2s).
- **Communication:** RESTful API (UnityWebRequest), JSON.

## Project Structure

- `virtual_museum/`: The Unity project source files.
- `main.py`: FastAPI server entry point.
- `agent_orchestrator.py`: Core multi-agent logic for the Gemini-powered orchestrator.
- `AI_Museum.pdf`: Full technical presentation and architectural deep-dive of the project.
- `museum_data.json`: Knowledge base containing exhibit details, history, and locations.
- `requirements.txt`: Python backend dependencies.

## Setup & Installation

### Backend Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Liza228ko/llm-virtual-museum-guide.git
   cd virtual-museum
   ```

2. **Create a virtual environment:**
   ```bash
   python3 -m venv venv
   source venv/bin/activate
   ```

3. **Install dependencies:**
   ```bash
   pip install -r requirements.txt
   ```

4. **Set up environment variables:**
   Create a `.env` file in the root directory:
   ```env
   GEMINI_API_KEY="YOUR_ACTUAL_API_KEY"
   ```

5. **Run the server:**
   ```bash
   uvicorn main:app --host 0.0.0.0 --port 8000
   ```

### Unity Setup

1. Open the `virtual_museum` folder in **Unity Hub** (Recommended version: 6000.0.58f1).
2. Ensure the `BackendUrl` in `Assets/Scripts/GeminiController.cs` points to your running server (default is `http://localhost:8000/ask`).
3. Press **Play** to start the simulation.

## Future Work

- **Model Context Protocol (MCP) Integration** 
- **Agent Development Kit (ADK)**

---
*Developed by Yelyzaveta Kozachenko for the NYCU Beyond Computing Museum.*
