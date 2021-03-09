using Newtonsoft.Json;
using System.Collections.Generic;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Single Output for a Node
    /// </summary>
    public class NodeOutput
    {
        private readonly Node _node;

        /// <summary>
        /// Expression of the character
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Character that says the text
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// Text for the output
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Whether output should still be available after returning to the node
        /// </summary>
        public bool KeepEnabled { get; set; }

        /// <summary>
        /// Condition for the output to be visible
        /// </summary>
        public string Condition { get; private set; }

        /// <summary>
        /// Event id to trigger
        /// </summary>
        public int Event;

        /// <summary>
        /// The followup node
        /// </summary>
        public Node Output { get; private set; }

        public NodeOutput(Node node)
        {
            _node = node;
        }

        /// <summary>
        /// Sets the condition
        /// </summary>
        /// <param name="condition">New Condition</param>
        /// <returns>Whether the condition is valid</returns>
        public bool SetCondition(string condition)
        {
            if(string.IsNullOrWhiteSpace(condition))
            {
                Condition = "";
                return true;
            }
            if(condition == ":")
            {
                Condition = condition;
                return true;
            }

            int depth = 0;
            char last = '(';

            foreach(char c in condition)
            {
                switch(c)
                {
                    case '(':
                        if(char.IsNumber(last) || last == '-' || last == ')')
                            return false;
                        depth++;
                        break;
                    case ')':
                        if(depth < 0 || !(char.IsNumber(last) || last == ')'))
                            return false;
                        depth--;
                        break;
                    case '!':
                        if(!(last == '(' || last == '|' || last == '&' || last == '!' || last == '-'))
                            return false;
                        break;
                    case '-':
                        if(!(last == '(' || last == '|' || last == '&' || last == '-'))
                            return false;
                        break;
                    case '|':
                    case '&':
                        if(!char.IsNumber(last) && last != ')')
                            return false;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if(last == ')')
                            return false;
                        break;
                    default:
                        return false;
                }
                last = c;
            }
            Condition = condition;
            return true;
        }

        public bool SetOutput(Node newOutput)
        {
            if(newOutput == null)
            {
                if(Output != null)
                    Output.Inputs.Remove(this);
                Output = null;
            }
            else if(newOutput.Outputs.Contains(this))
                return false;

            if(Output != null)
                Output.Inputs.Remove(this);

            Output = newOutput;
            Output?.Inputs.Add(this);
            return true;
        }

        public void Delete() => _node.RemoveOutput(this);

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Expression");
            writer.WriteValue(Expression);

            writer.WritePropertyName("Character");
            writer.WriteValue(Character);

            writer.WritePropertyName("Text");
            writer.WriteValue(Text);

            writer.WritePropertyName("KeepEnabled");
            writer.WriteValue(KeepEnabled);

            writer.WritePropertyName("Condition");
            writer.WriteValue(Condition);

            writer.WritePropertyName("Event");
            writer.WriteValue(Event);

            writer.WritePropertyName("Output");
            writer.WriteValue(Output?.Index);

            writer.WriteEndObject();
        }

        public static NodeOutput ReadJson(JsonReader reader, Node node, Dictionary<NodeOutput, int> outputIndices)
        {
            NodeOutput result = new(node);

            while(reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string tokenName = (string)reader.Value;
                    reader.Read();
                    switch(tokenName)
                    {
                        case "Expression":
                            result.Expression = (string)reader.Value;
                            break;
                        case "Character":
                            result.Character = (string)reader.Value;
                            break;
                        case "Text":
                            result.Text = (string)reader.Value;
                            break;
                        case "KeepEnabled":
                            result.KeepEnabled = (bool)reader.Value;
                            break;
                        case "Condition":
                            result.Condition = (string)reader.Value;
                            break;
                        case "Event":
                            result.Event = (int)(long)reader.Value;
                            break;
                        case "Output":
                            if(reader.Value != null)
                                outputIndices.Add(result, (int)(long)reader.Value);
                            break;

                    }
                }
            }

            return result;
        }
    }
}