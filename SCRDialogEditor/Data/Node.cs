using System.Collections.Generic;
using Newtonsoft.Json;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Single node of a dialog
    /// </summary>
    public class Node
    {
        private readonly Dialog _dialog;

        public int Index => _dialog.Nodes.IndexOf(this);

        public int LocationX { get; set; }
        public int LocationY { get; set; }

        /// <summary>
        /// Input references
        /// </summary>
        public List<NodeOutput> Inputs { get; }

        /// <summary>
        /// Output Sockets
        /// </summary>
        public List<NodeOutput> Outputs { get; }


        public Node(Dialog dialog)
        {
            _dialog = dialog;
            Inputs = new();
            Outputs = new()
            {
                // default output
                new NodeOutput(this)
            };
        }

        /// <summary>
        /// Deletes the node from the dialog
        /// </summary>
        public void Delete()
        {
            foreach(NodeOutput no in Inputs)
                no.SetOutput(null);
            foreach(NodeOutput no in Outputs)
                no.SetOutput(null);
            _dialog.Nodes.Remove(this);
            if(_dialog.StartNode == this)
                _dialog.StartNode = null;
        }

        /// <summary>
        /// Adds output to the Node
        /// </summary>
        /// <returns></returns>
        public NodeOutput AddOutput()
        {
            NodeOutput result = new NodeOutput(this)
            {
                Expression = Outputs[0].Expression,
                Character = Outputs[0].Character
            };
            Outputs.Add(result);
            return result;
        }

        /// <summary>
        /// Removes a specific output
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool RemoveOutput(NodeOutput output)
        {
            if(Outputs.Count < 2)
                return false;

            if(!Outputs.Contains(output))
                return false;

            Outputs.Remove(output);
            output.SetOutput(null);
            return true;
        }

        public void WriteJSON(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("LocationX");
            writer.WriteValue(LocationX);

            writer.WritePropertyName("LocationY");
            writer.WriteValue(LocationY);

            writer.WritePropertyName("Outputs");
            writer.WriteStartArray();

            foreach(NodeOutput no in Outputs)
                no.WriteJson(writer);

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public static Node ReadJson(JsonReader reader, Dialog dialog, Dictionary<NodeOutput, int> outputIndices)
        {
            Node result = new(dialog);

            while(reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string tokenName = (string)reader.Value;
                    reader.Read();
                    switch(tokenName)
                    {
                        case "LocationX":
                            result.LocationX = (int)(long)reader.Value;
                            break;
                        case "LocationY":
                            result.LocationY = (int)(long)reader.Value;
                            break;
                        case "Outputs":
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                result.Outputs.Add(NodeOutput.ReadJson(reader, result, outputIndices));
                            }
                            break;
                    }
                }
            }

            result.RemoveOutput(result.Outputs[0]);

            return result;
        }
    }
}
