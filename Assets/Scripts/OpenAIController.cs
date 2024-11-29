using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
	public TMP_Text textField;
	public TMP_InputField inputField;
	public Button okbutton;
	
	private OpenAIAPI api;
	private List<ChatMessage> messages;
	
	void Start()
	{
		api = new OpenAIAPI("You OpenAI_APIKey!");
		if(api != null)
		{
			Debug.Log(api.ApiUrlFormat);
		}
		
		StartConversation();
		okbutton.onClick.AddListener(()=> GetResponse());
	}
	
	private void StartConversation()
	{
		messages = new List<ChatMessage>
		{
			new ChatMessage(ChatMessageRole.System,"You are an honorable, friendly Knight guarding the gate to the palace. You will only allow someone who knows the secret password to enter. The password is magic")
		};
		
		inputField.text = "";
		string startString = "You have just approached the palace gate where a knight guards the gate.";
		textField.text = startString;
	}
	private async void GetResponse()
	{
		if(inputField.text.Length < 1)
		{
			return;
		}
		
		// Disable the OK button 
		okbutton.enabled = false;
		
		// Fill the user message from the input field.
		ChatMessage userMessage = new ChatMessage();
		userMessage.Role = ChatMessageRole.User;
		userMessage.TextContent = inputField.text;
		if(userMessage.TextContent.Length > 100)
		{
			// Limit messages to 100 characters.
			userMessage.TextContent = userMessage.TextContent.Substring(0,100);
			
		}
		
		Debug.Log(string.Format("{0}:{1}",userMessage.rawRole,userMessage.TextContent));
		// Add the message to the list
		messages.Add(userMessage);
		
		// update the text field with the user message
		textField.text = string.Format("You:{0}",userMessage.TextContent);
		
		// clear the inputfield.
		inputField.text = "";
		
		// Send the entire chat to OpenAI to get the next message.
		var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
		{
			Model = Model.ChatGPTTurbo_1106,
			Temperature = 0,
			MaxTokens = 50,
			Messages = messages
		});
		
		// Get the response message.
		ChatMessage responseMessgae = new ChatMessage();
		responseMessgae.Role = chatResult.Choices[0].Message.Role;
		responseMessgae.TextContent = chatResult.Choices[0].Message.TextContent;
		Debug.Log(string.Format("{0},{1}",responseMessgae.rawRole,responseMessgae.TextContent));
		
		// Add the response to the list of messgaes.
		messages.Add(responseMessgae);
		
		// Update the textField with the response.
		textField.text = string.Format("You:{0}\n\nGuard:{1}",userMessage.TextContent,responseMessgae.TextContent);
		
		okbutton.enabled = true;
	}
	
}
