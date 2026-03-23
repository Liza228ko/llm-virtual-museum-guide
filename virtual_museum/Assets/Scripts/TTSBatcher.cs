using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TTSBatcher : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Wit.ai works best with chunks under 140-160 characters.")]
    public int maxCharLimit = 140;

    [Header("References")]
    public TTSSpeaker ttsSpeaker; 

    /// <summary>
    /// Call this from GeminiController's OnResponse event.
    /// </summary>
    public void SpeakQueued(string fullText)
    {
        if (string.IsNullOrEmpty(fullText) || !ttsSpeaker) return;

        // 1. Split into Sentences first (Split by . ? ! but keep the punctuation)
        string[] sentences = Regex.Split(fullText, @"(?<=[.!?])\s+");

        foreach (string sentence in sentences)
        {
            string cleanSentence = sentence.Trim();
            if (string.IsNullOrEmpty(cleanSentence)) continue;

            // 2. Check if sentence is too long
            if (cleanSentence.Length <= maxCharLimit)
            {
                ttsSpeaker.SpeakQueued(cleanSentence);
            }
            else
            {
                // 3. If too long, break it down further by nearest whitespace
                QueueLongSentence(cleanSentence);
            }
        }
    }

    private void QueueLongSentence(string longText)
    {
        // Loop until the remaining text fits in the limit
        while (longText.Length > maxCharLimit)
        {
            // Find the last space within the limit (to avoid cutting words)
            int splitIndex = longText.LastIndexOf(' ', maxCharLimit);

            // Edge case: If there are no spaces (one giant word), force cut at limit
            if (splitIndex == -1) 
            {
                splitIndex = maxCharLimit;
            }

            // Extract the chunk and queue it
            string chunk = longText.Substring(0, splitIndex).Trim();
            ttsSpeaker.SpeakQueued(chunk);

            // Remove the chunk from the original text and repeat
            longText = longText.Substring(splitIndex).Trim();
        }

        // Queue the final remainder
        if (!string.IsNullOrEmpty(longText))
        {
            ttsSpeaker.SpeakQueued(longText);
        }
    }
}