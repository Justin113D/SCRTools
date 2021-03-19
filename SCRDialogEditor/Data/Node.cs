using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Single node of a dialog
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Outputs list
        /// </summary>
        private readonly List<NodeOutput> _outputs;

        /// <summary>
        /// Inputs list
        /// </summary>
        private readonly List<NodeOutput> _inputs;

        /// <summary>
        /// Grid location on the X Axis
        /// </summary>
        public int LocationX { get; set; }

        /// <summary>
        /// Grid location on the Y Axis
        /// </summary>
        public int LocationY { get; set; }

        /// <summary>
        /// If true, the portrait will be placed/focused on the right. Else on the left
        /// </summary>
        public bool RightPortrait { get; set; }

        /// <summary>
        /// Input references
        /// </summary>
        public ReadOnlyCollection<NodeOutput> Inputs { get; }

        /// <summary>
        /// Output Sockets
        /// </summary>
        public ReadOnlyCollection<NodeOutput> Outputs { get; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public Node()
        {
            _outputs = new()
            {
                new()
            };
            _inputs = new();

            Outputs = _outputs.AsReadOnly();
            Inputs = _inputs.AsReadOnly();
        }


        /// <summary>
        /// Deletes the node from the dialog
        /// </summary>
        public void Disconnect()
        {
            foreach(NodeOutput no in _inputs)
                no.Disconnect();
            foreach(NodeOutput no in _outputs)
                no.Disconnect();
        }

        /// <summary>
        /// Adds output to the Node
        /// </summary>
        /// <returns></returns>
        public NodeOutput CreateOutput()
        {
            NodeOutput result = new()
            {
                Expression = Outputs[0].Expression,
                Character = Outputs[0].Character
            };
            _outputs.Add(result);
            return result;
        }

        /// <summary>
        /// Removes a specific output
        /// </summary>
        /// <param name="nodeOutput"></param>
        /// <returns></returns>
        public bool RemoveOutput(NodeOutput nodeOutput)
        {
            if(_outputs.Count < 2
               || !_outputs.Contains(nodeOutput))
                return false;

            nodeOutput.Disconnect();
            _outputs.Remove(nodeOutput);
            return true;
        }

        /// <summary>
        /// Registers a connected input
        /// </summary>
        /// <param name="nodeOutput"></param>
        public void AddInput(NodeOutput nodeOutput)
            => _inputs.Add(nodeOutput);

        /// <summary>
        /// Deregisters a connected input
        /// </summary>
        /// <param name="nodeOutput"></param>
        public void RemoveInput(NodeOutput nodeOutput)
            => _inputs.Remove(nodeOutput);

        /// <summary>
        /// Writes the Node to a json stream
        /// </summary>
        public void WriteJSON(JsonWriter writer, Dialog dialog)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("LocationX");
            writer.WriteValue(LocationX);

            writer.WritePropertyName("LocationY");
            writer.WriteValue(LocationY);

            writer.WritePropertyName("Outputs");
            writer.WriteStartArray();

            foreach(NodeOutput no in Outputs)
                no.WriteJson(writer, dialog);

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        /// <summary>
        /// Reads the node from a json stream
        /// </summary>
        /// <param name="reader">The json source reader</param>
        /// <param name="outputIndices"></param>
        /// <returns></returns>
        public static Node ReadJson(JsonReader reader, Dictionary<NodeOutput, int> outputIndices)
        {
            Node result = new();

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
                                result._outputs.Add(NodeOutput.ReadJson(reader, outputIndices));
                            break;
                    }
                }
            }

            result.RemoveOutput(result.Outputs[0]);

            return result;
        }

        public override string ToString()
            => $"Loc ({LocationX}, {LocationY}); Out: {_outputs.Count}, In: {_inputs.Count}";
    }
}
