using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// hi kirk. i can code too lololololololololololololololololololzzzzzzzz


namespace DemoDurb
{
    public class NN
    {
        public List<net> networks = new List<net>();

        public void create(int inputs, int hidden_layers,int neurons_per_layer, int outputs)
        {
            net temp = new net(true);

            temp.add(new layer(inputs, 1, layerType.input)); //create input layer

            for (int i = 0; i!=hidden_layers; i++)
            {
                temp.add(new layer(neurons_per_layer, temp.layers.Count, layerType.hidden)); //create hidden layers
            }

            temp.add(new layer(outputs, temp.layers.Count, layerType.output)); //create output layer

            networks.Add(temp);
        }

        public static float[] update(net _net,float[] inputs)
        {
            float[] _out = new float[_net.layers[_net.layers.Count-1].nurons.Length];

            #region InputNewData
            for (int i = 0; i != inputs.Length; i++)
            {
                if (_net.layers[0].type == layerType.input)          //be sure that we are addin input to the input layer
                    _net.layers[0].nurons[i].inputs[0].value = inputs[i];  //add input to each neuron's first and only input;
            } 
            #endregion

            for(int l = 0; l!= _net.layers.Count;l++)
            {
                    for (int n = 0; n != _net.layers[l].nurons.Length; n++)
                    {
                        float neuron_out = inputs_process(_net.layers[l].nurons[n].inputs);
                        if (_net.layers[l].type != layerType.output)
                        {
                            for (int o = 0; o != _net.layers[l + 1].nurons.Length; o++)
                            {
                                _net.layers[l + 1].nurons[o].inputs[n].value = neuron_out;
                            }
                        }
                        else
                        {
                            //for (int o = 0; o != _out.Length; o++)
                            //{
                                _out[n] = neuron_out;
                            //}
                        }
                    }
                }
            return _out;
        }

        #region NeuronShit
        static float inputs_process(input[] inputs)
        {
            float _out = (float)(1 / (1 + Math.Exp(-inputs_sum(inputs) / 1)));
            return _out;
        }
        static float inputs_sum(input[] inputs)
        {
            float sum = 0; //activation

            for (int i = 0; i != inputs.Length; i++)
            {
                sum += (inputs[i].value * inputs[i].weight);
            }

            return sum;
        }
        #endregion

        #region StructuresAndEnumerators

        public struct input
        {
            public float value;
            public float weight;

            public input(float Value, float Weight)
            {
                value = Value;
                weight = Weight;
            }
        }
        public struct neuron
        {
            public input[] inputs;

            public neuron(int inputs_num)
            {
                inputs = new input[inputs_num];

                for (int i = 0; i != inputs_num; i++)
                {
                    inputs[i].weight = RandomNumber(-1, 1); //assign random weights
                    inputs[i].value = 0;
                }
            }
        }
        public struct layer
        {
            public neuron[] nurons;
            public layerType type;

            public layer(int nurons_num, int inputs, layerType Type)
            {
                nurons = new neuron[nurons_num];
                type = Type;

                for (int i = 0; i != nurons_num; i++)
                {
                    nurons[i] = new neuron(inputs);  //inputs are the number of nurons in the previous layer
                }
            }
        }
        public enum layerType
        { input, hidden, output }; 
        #endregion

        #region RandomShit
        public static float RandomNumber(double min, double max)
        {
            float OUT = (float)((max - min) * m_Rand.NextDouble() + min);
            return OUT;
        }
        private static Random m_Rand = new Random(); 
        #endregion
    }
    public class net
    {
        public float fitness;
            public List<NN.layer> layers = new List<NN.layer>();

            public net(bool clear)
            {
                fitness = 0;
                if(clear)
                layers.Clear();
            }
            public void add(NN.layer layer)
            {
                layers.Add(layer);
            }
            public net mutate(net network)
            {
                for(int l = 0;l!=network.layers.Count;l++)
                {
                    for(int n=0;n !=network.layers[l].nurons.Length;n++)
                    {
                        for (int i = 0; i != network.layers[l].nurons[n].inputs.Length; i++)
                        {
                            NN.neuron neuron = network.layers[l].nurons[n];
                            neuron.inputs[i].weight = NN.RandomNumber(neuron.inputs[i].weight - 1, neuron.inputs[i].weight + 1);
                            network.layers[l].nurons[n] = neuron;
                        }
                    }
                }
                return network;
            }


    }
}
