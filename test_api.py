import requests

url = "http://127.0.0.1:8000/ask"
data = {"query": "Show me a supercomputer"}

try:
    response = requests.post(url, data=data)
    response.raise_for_status()  # Raise an exception for bad status codes (4xx or 5xx)
    print("Test successful! Response:")
    print(response.json())
except requests.exceptions.RequestException as e:
    print(f"An error occurred: {e}")
