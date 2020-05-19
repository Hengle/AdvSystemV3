// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
    /// <summary>
    /// Parses a string for special Fungus text tags.
    /// </summary>
    public static class TextTagParserExtend
    {
        const string TextTokenRegexString = @"\{.*?\}";

        private static void AddWordsToken(List<TextTagTokenExtend> tokenList, string words)
        {
            TextTagTokenExtend token = new TextTagTokenExtend();
            token.type = TokenTypeExtend.Words;
            token.paramList = new List<string>(); 
            token.paramList.Add(words);
            tokenList.Add(token);
        }
        
        private static void AddTagToken(List<TextTagTokenExtend> tokenList, string tagText)
        {
            if (tagText.Length < 3 ||
                tagText.Substring(0,1) != "{" ||
                tagText.Substring(tagText.Length - 1,1) != "}")
            {
                return;
            }
            
            string tag = tagText.Substring(1, tagText.Length - 2);
            
            var type = TokenTypeExtend.Invalid;
            List<string> parameters = ExtractParameters(tag);
            
            if (tag == "b")
            {
                type = TokenTypeExtend.BoldStart;
            }
            else if (tag == "/b")
            {
                type = TokenTypeExtend.BoldEnd;
            }
            else if (tag == "i")
            {
                type = TokenTypeExtend.ItalicStart;
            }
            else if (tag == "/i")
            {
                type = TokenTypeExtend.ItalicEnd;
            }
            else if (tag.StartsWith("color="))
            {
                type = TokenTypeExtend.ColorStart;
            }
            else if (tag == "/color")
            {
                type = TokenTypeExtend.ColorEnd;
            }
            else if (tag.StartsWith("size="))
            {
                type = TokenTypeExtend.SizeStart;
            }
            else if (tag == "/size")
            {
                type = TokenTypeExtend.SizeEnd;
            }
            else if (tag == "wi")
            {
                type = TokenTypeExtend.WaitForInputNoClear;
            }
            else if (tag == "wc")
            {
                type = TokenTypeExtend.WaitForInputAndClear;
            }
            else if (tag == "wvo")
            {
                type = TokenTypeExtend.WaitForVoiceOver;
            }
            else if (tag.StartsWith("wp="))
            {
                type = TokenTypeExtend.WaitOnPunctuationStart;
            }
            else if (tag == "wp")
            {
                type = TokenTypeExtend.WaitOnPunctuationStart;
            }
            else if (tag == "/wp")
            {
                type = TokenTypeExtend.WaitOnPunctuationEnd;
            }
            else if (tag.StartsWith("w="))
            {
                type = TokenTypeExtend.Wait;
            }
            else if (tag == "w")
            {
                type = TokenTypeExtend.Wait;
            }
            else if (tag == "c")
            {
                type = TokenTypeExtend.Clear;
            }
            else if (tag.StartsWith("s="))
            {
                type = TokenTypeExtend.SpeedStart;
            }
            else if (tag == "s")
            {
                type = TokenTypeExtend.SpeedStart;
            }
            else if (tag == "/s")
            {
                type = TokenTypeExtend.SpeedEnd;
            }
            else if (tag == "x")
            {
                type = TokenTypeExtend.Exit;
            }
            else if (tag.StartsWith("m="))
            {
                type = TokenTypeExtend.Message;
            }
            //Sora.add
            else if (tag.StartsWith("emoji="))
            {
                type = TokenTypeExtend.Emoji;
            }
            else if (tag.StartsWith("vpunch") ||
                     tag.StartsWith("vpunch="))
            {
                type = TokenTypeExtend.VerticalPunch;
            }
            else if (tag.StartsWith("hpunch") ||
                     tag.StartsWith("hpunch="))
            {
                type = TokenTypeExtend.HorizontalPunch;
            }
            else if (tag.StartsWith("punch") ||
                     tag.StartsWith("punch="))
            {
                type = TokenTypeExtend.Punch;
            }
            else if (tag.StartsWith("flash") ||
                     tag.StartsWith("flash="))
            {
                type = TokenTypeExtend.Flash;
            }
            else if (tag.StartsWith("audio="))
            {
                type = TokenTypeExtend.Audio;
            }
            else if (tag.StartsWith("audioloop="))
            {
                type = TokenTypeExtend.AudioLoop;
            }
            else if (tag.StartsWith("audiopause="))
            {
                type = TokenTypeExtend.AudioPause;
            }
            else if (tag.StartsWith("audiostop="))
            {
                type = TokenTypeExtend.AudioStop;
            }
            
            if (type != TokenTypeExtend.Invalid)
            {
                TextTagTokenExtend token = new TextTagTokenExtend();
                token.type = type;
                token.paramList = parameters;           
                tokenList.Add(token);
            }
            else
            {
                Debug.LogWarning("Invalid text tag " + tag);
            }
        }

        private static List<string> ExtractParameters(string input)
        {
            List<string> paramsList = new List<string>();
            int index = input.IndexOf('=');
            if (index == -1)
            {
                return paramsList;
            }

            string paramsStr = input.Substring(index + 1);
            var splits = paramsStr.Split(',');
            for (int i = 0; i < splits.Length; i++)
            {
                var p = splits[i];
                paramsList.Add(p.Trim());
            }
            return paramsList;
        }

        #region Public members

        /// <summary>
        /// Returns a description of the supported tags.
        /// </summary>
        public static string GetTagHelp()
        {
            return "" +
                "\t{b} Bold Text {/b}\n" + 
                "\t{i} Italic Text {/i}\n" +
                "\t{color=red} Color Text (color){/color}\n" +
                "\t{size=30} Text size {/size}\n" +
                "\n" +
                "\t{s}, {s=60} Writing speed (chars per sec){/s}\n" +
                "\t{w}, {w=0.5} Wait (seconds)\n" +
                "\t{wi} Wait for input\n" +
                "\t{wc} Wait for input and clear\n" +
                "\t{wvo} Wait for voice over line to complete\n" +
                "\t{wp}, {wp=0.5} Wait on punctuation (seconds){/wp}\n" +
                "\t{c} Clear\n" +
                "\t{x} Exit, advance to the next command without waiting for input\n" +
                "\n" +
                "\t{emoji=Key} Switch emoji (character emoji key)\n" +
                "\t{vpunch=10,0.5} Vertically punch screen (intensity,time)\n" +
                "\t{hpunch=10,0.5} Horizontally punch screen (intensity,time)\n" +
                "\t{punch=10,0.5} Punch screen (intensity,time)\n" +
                "\t{flash=0.5} Flash screen (duration)\n" +
                "\n" +
                "\t{audio=AudioObjectName} Play Audio Once\n" +
                "\t{audioloop=AudioObjectName} Play Audio Loop\n" +
                "\t{audiopause=AudioObjectName} Pause Audio\n" +
                "\t{audiostop=AudioObjectName} Stop Audio\n" +
                "\n" +
                "\t{m=MessageName} Broadcast message\n" +
                "\t{$VarName} Substitute variable";
        }

        /// <summary>
        /// Processes a block of story text and converts it to a list of tokens.
        /// </summary>
        public static List<TextTagTokenExtend> Tokenize(string storyText)
        {
            List<TextTagTokenExtend> tokens = new List<TextTagTokenExtend>();

            Regex myRegex = new Regex(TextTokenRegexString);

            Match m = myRegex.Match(storyText);   // m is the first match

            int position = 0;
            while (m.Success)
            {
                // Get bit leading up to tag
                string preText = storyText.Substring(position, m.Index - position);
                string tagText = m.Value;

                if (preText != "")
                {
                    AddWordsToken(tokens, preText);
                }
                AddTagToken(tokens, tagText);

                position = m.Index + tagText.Length;
                m = m.NextMatch();
            }

            if (position < storyText.Length)
            {
                string postText = storyText.Substring(position, storyText.Length - position);
                if (postText.Length > 0)
                {
                    AddWordsToken(tokens, postText);
                }
            }

            // Remove all leading whitespace & newlines after a {c} or {wc} tag
            // These characters are usually added for legibility when editing, but are not 
            // desireable when viewing the text in game.
            bool trimLeading = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (trimLeading && token.type == TokenTypeExtend.Words)
                {
                    token.paramList[0] = token.paramList[0].TrimStart(' ', '\t', '\r', '\n');
                }
                if (token.type == TokenTypeExtend.Clear || token.type == TokenTypeExtend.WaitForInputAndClear)
                {
                    trimLeading = true;
                }
                else
                {
                    trimLeading = false;
                }
            }

            return tokens;
        }

        #endregion
    }    
}