using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Connect4NNGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button[] spot;
        NeuralNetworkOwn nn;
        Connect4 c;
        public MainWindow()
        {
            InitializeComponent();
          

            Canvas gameGrid = (Canvas)FindName("gameCanvas");
            double width = grid.ActualWidth;
            double height = grid.ActualHeight;
            grid.SizeChanged += Grid_SizeChanged;
            spot = new Button[42];
            for (int i = 0; i < 42; i++)
            {
                spot[i] = new Button();
                spot[i].Width = width / 7;
                spot[i].Height = height / 6;
               
                spot[i].Tag = i.ToString();
                spot[i].SetValue(Canvas.LeftProperty, ((7 - ((42 - i) % 7))) * (width / 7));
                spot[i].SetValue(Canvas.TopProperty, ((42 - i) / 7) * (height / 6));
                spot[i].Click += MainWindow_Click;
                gameGrid.Children.Add(spot[i]);
            }
            Grid_SizeChanged(grid, null);
            c = new Connect4();
            nn = new NeuralNetworkOwn(42, 2, 70, 42);
            UpdateColors();
        }

      

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = grid.ActualWidth;
            double height = grid.ActualHeight;
            for (int i = 0; i < 42; i++)
            {
                spot[i].Width = width / 7;
                spot[i].Height = height / 6;
                if(7 - ((42 - i) % 7)== 7){
                    spot[i].SetValue(Canvas.LeftProperty, ((42 - i) % 7) *(width / 7));

                }
                else
                {
                    spot[i].SetValue(Canvas.LeftProperty, ((7 - ((42 - i) % 7))) * (width / 7));

                }
                if ((42 - i) % 7 == 0)
                {
                    spot[i].SetValue(Canvas.TopProperty, (((42 - i) / 7)-1) * (height / 6));

                }
                else
                {

                    spot[i].SetValue(Canvas.TopProperty, ((42 - i) / 7) * (height / 6));
                }
            }
        }


        private void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            int buttonPressed =Convert.ToInt16( clicked.Tag);
            checkMove(buttonPressed);
        }

   
        private void checkMove(int butPressed)
        {
            bool[] spacesUsable = c.gameCheckPeices();
            double[] locs = new double[42];
            locs[butPressed] = 1;

            if (spacesUsable[butPressed])
            {
                c.makeMove(true, locs, spacesUsable);
                UpdateColors();
                int result = c.checkWin();
                //train using c as the input if zero do nothing scratch, if one then adjust for the win, if two then adjust for player two winning

                //below can be deleted especially considering it may become messy.
                if (result == -1)
                {
                    Thread.Sleep(3000);
                    nn.trainNeuralNetwork(result, c.getUsedBy());
                    gameWon(1);

                }
                else if (result == 1)
                {
                    //computer won
                    Thread.Sleep(3000);

                    nn.trainNeuralNetwork(result, c.getUsedBy());
                    gameWon(2);
                }
                else if (result == -2)
                {
                    UpdateColors();
                    nnMakeMove();

                }
                else
                {
                    //scratch
                    Thread.Sleep(3000);

                    gameWon(0);
                }
            }
        }
        private void gameWon(int result)
        {
            replayWindow win2 = new replayWindow();
            Label res = (Label)win2.FindName("resultLab");
            switch (result)
            {
                case 0:
                    res.Content = res.Content + " Tie";
                    break;
                case 1:
                    res.Content = res.Content + "You Won";

                    break;
                case 2:
                    res.Content = res.Content + "The Future Won";

                    break;
            }
            win2.Show();
            this.Hide();
            Button yes = (Button)win2.FindName("yesButt");
            yes.Click += delegate {
                this.Show();
                restartProgram();
                win2.Hide();
            };
            Button no = (Button)win2.FindName("noButt");
            no.Click += delegate {
                System.Windows.Application.Current.Shutdown();

            };

        }
        public void restartProgram()
        {


            Canvas gameGrid = (Canvas)FindName("gameCanvas");
            gameGrid.Children.Clear();
            double width = grid.ActualWidth;
            double height = grid.ActualHeight;
            grid.SizeChanged += Grid_SizeChanged;
            spot = new Button[42];
            for (int i = 0; i < 42; i++)
            {
                spot[i] = new Button();
                spot[i].Width = width / 7;
                spot[i].Height = height / 6;
               
                spot[i].Tag = i.ToString();
                spot[i].SetValue(Canvas.LeftProperty, ((7 - ((42 - i) % 7))) * (width / 7));
                spot[i].SetValue(Canvas.TopProperty, ((42 - i) / 7) * (height / 6));
                spot[i].Click += MainWindow_Click;
                
                gameGrid.Children.Add(spot[i]);
            }
            Grid_SizeChanged(grid,null);
            c = new Connect4();
            nn = new NeuralNetworkOwn(42, 2, 70, 42);
            UpdateColors();
        }
        private void nnMakeMove()
        {
            c.makeMove(false, nn.getMoveValues(c.getUsedBy(), true), c.gameCheckPeices());
            int result = c.checkWin();
            //train using c as the input if zero do nothing scratch, if one then adjust for the win, if two then adjust for player two winning

            //below can be deleted especially considering it may become messy.
            if (result == -1)
            {
                nn.trainNeuralNetwork(result, c.getUsedBy());
                gameWon(1);

            }
            else if (result == 1)
            {
                //computer won
                nn.trainNeuralNetwork(result, c.getUsedBy());
                gameWon(2);
            }
            else if (result==-2)
            {
                UpdateColors();
            }
            else
            {
                //scratch
                gameWon(0);
            }
        }
        private void UpdateColors()
        {
           bool[] spacesUsable= c.gameCheckPeices();
            int[] usedBy = c.getUsedBy();
               
            for (int i = 0; i < 42; i++)
            {

                Button b = spot[i];
                b.Background = new SolidColorBrush(Colors.Black);
                b.Foreground = new SolidColorBrush(Colors.Black);
                if (spacesUsable[i])
                {
                    b.Background = new SolidColorBrush(Colors.White);
                    b.Foreground = new SolidColorBrush(Colors.White);
                }
                if (usedBy[i] == -1)
                {
                    b.Background = new SolidColorBrush(Colors.Blue);
                    b.Foreground = new SolidColorBrush(Colors.Blue);
                }
                if (usedBy[i] == 1)
                {
                    b.Background = new SolidColorBrush(Colors.Red);
                    b.Foreground = new SolidColorBrush(Colors.Red);
                }

            }

        }
    }
    public class Connect4
    {
        bool[] spacesUsed;
        bool[] spacesUsable;
        int[] usedBy;
        int fails;
        public int getFails()
        {
            return fails;
        }
        public Connect4()
        {
            fails = 0;
            spacesUsed = new bool[42];
            spacesUsable = new bool[42];
            usedBy = new int[42];
            for (int i = 0; i < 42; i++)
            {
                spacesUsed[i] = false;
                spacesUsable[i] = false;
                //0,1,2
                usedBy[i] = 0;
            }
        }

        public int[] getUsedBy()
        {
            return usedBy;
        }
        public bool[] gameCheckPeices()
        {
            for (int i = 0; i < 42; i++)
            {
                spacesUsable[i] = false;
            }
            for (int i = 0; i < 7; i++)
            {
                if (!spacesUsed[i])
                {
                    spacesUsable[i] = true;

                }
                else
                {
                    int multiplier = 0;
                    bool complete = false;
                    //make sure this line wont cause a crash due to overflowing array.
                    try
                    {
                        while (spacesUsed[i + (7 * multiplier)] || complete)
                        {
                            spacesUsable[i + (7 * multiplier)] = false;
                            multiplier += 1;
                            if (i + (7 * multiplier) < 42)
                            {
                                spacesUsable[i + (7 * multiplier)] = true;
                            }
                            else
                            {
                                complete = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing, only crashed because spaces used exceeded bounds

                        fails += 1;
                    }
                }
            }
            return spacesUsable;
        }

        public int checkWin()
        {
            bool noZeros = true;
            for (int i = 0; i < 42; i++)
            {
                if (usedBy[i] != 0)
                {
                    //check diagnol and vertical
                    if (i < 21)
                    {
                        //vert
                        if (usedBy[i] == usedBy[i + 7] && usedBy[i + 7] == usedBy[i + 14] && usedBy[i + 14] == usedBy[i + 21])
                        {
                            return usedBy[i];
                        }
                        switch (i % 7)
                        {
                            case 0:
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }
                                break;
                            case 1:
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }
                                break;
                            case 2:
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }
                                break;
                            case 3:
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }

                                break;
                            case 4:
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];

                                    }

                                }
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                break;
                            case 5:
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                break;
                            case 6:
                                if (i < 18)
                                {
                                    if (usedBy[i] == usedBy[i + 8] && usedBy[i + 8] == usedBy[i + 16] && usedBy[i + 16] == usedBy[i + 24])
                                    {
                                        //won diag
                                        return usedBy[i];
                                    }
                                }
                                if (usedBy[i] == usedBy[i + 6] && usedBy[i + 6] == usedBy[i + 12] && usedBy[i + 12] == usedBy[i + 18])
                                {
                                    //won diag
                                    return usedBy[i];
                                }
                                break;
                        }
                    }
                    if (i < 35)
                    {
                        //check horizontal
                        switch (i % 7)
                        {
                            case 0:
                                if (usedBy[i] == usedBy[i + 1] && usedBy[i + 1] == usedBy[i + 2] && usedBy[i + 2] == usedBy[i + 3])
                                {
                                    return usedBy[i];

                                }
                                break;
                            case 1:
                                if (usedBy[i] == usedBy[i + 1] && usedBy[i + 1] == usedBy[i + 2] && usedBy[i + 2] == usedBy[i + 3])
                                {
                                    return usedBy[i];

                                }
                                break;
                            case 2:
                                if (usedBy[i] == usedBy[i + 1] && usedBy[i + 1] == usedBy[i + 2] && usedBy[i + 2] == usedBy[i + 3])
                                {
                                    return usedBy[i];

                                }
                                break;
                            case 3:
                                if (usedBy[i] == usedBy[i + 1] && usedBy[i + 1] == usedBy[i + 2] && usedBy[i + 2] == usedBy[i + 3])
                                {
                                    return usedBy[i];
                                }
                                break;
                        }
                    }
                }
                else
                {
                    noZeros = false;
                }
            }
            if (noZeros)
            {
                //scratch
                return 0;
            }
            return -2;
        }
        public void makeMove(bool user, double[] locationsValues, bool[] spacesUse)
        {

            int location = 0;
            int index = 0;
            double maxLocValue = -2;
            foreach (float item in locationsValues)
            {
                if (item > maxLocValue && spacesUse[index])
                {
                    location = index;
                    maxLocValue = item;
                }
                index++;
            }
            if (user)
            {
                usedBy[location] = -1;
            }
            else
            {
                usedBy[location] = 1;
            }
            spacesUsed[location] = true;
            //guranteed to be move between -1 and 1 where unused spaced if not cat game
        }
    }


    //2 hiddne layers, 70 neurons each layer (2/3*42)+42
    public class NeuralNetworkOwn
    {
        List<Layer>[] layersOfNN;
        double learningRate = .033;
        List<Layer> layers;
        public NeuralNetworkOwn(int input, int hiddenLayers, int hiddenNeurons, int outputs)
        {
            if (getFromDatabase())
            {
                getFromDatabase();
                layersOfNN = new List<Layer>[2];
                layersOfNN[0] = layers;
                layersOfNN[1] = oldLayers;
            }
            else
            {
                layers = new List<Layer>();
                layers.Add(new Layer(input));
                for (int i = 0; i < hiddenLayers; i++)
                {
                    layers.Add(new Layer(hiddenNeurons));
                }
                layers.Add(new Layer(outputs));
                for (int i = 0; i < layers.Count; i++)
                {
                    for (int n = 0; n < layers[i].NeuronsNum; n++)
                    {
                        layers[i].Neurons.Add(new Neuron());



                    }
                    layers[i].Neurons.ForEach((neuron) =>
                    {
                        if (i == 0)
                        {
                            //no need to check file because should be no change from time to time that is not fixed above
                            neuron.Bias = 0;
                        }
                        else
                        {
                            for (int d = 0; d < layers[i - 1].NeuronCount; d++)
                            {
                                neuron.Dendrites.Add(new Dendrite());

                            }
                        }
                    });
                }
                oldLayers = layers;
                layersOfNN = new List<Layer>[2];
                layersOfNN[0] = layers;
                layersOfNN[1] = oldLayers;
            }
        }
        public double sigmoid(double x)
        {
            return (1 / (1 + Math.Exp(-x)));
        }
        public double[] getMoveValues(int[] usedBy, bool PlayerOne)
        {
            double[] inputs = new double[usedBy.Length];

            double[] outputs = new double[usedBy.Length];
            //assign weights
            for (int i = 0; i < usedBy.Length; i++)
            {
                inputs[i] = usedBy[i];
            }
            
            if (PlayerOne)
            {
                for (int i = 0; i < layersOfNN[0].Count; i++)
                {
                    Layer lay = layersOfNN[0][i];

                    for (int n = 0; n < lay.NeuronCount; n++)
                    {
                        Neuron neuron = lay.Neurons[n];
                        //sets first layer values to be input
                        if (i == 0)
                        {
                            neuron.Value = inputs[n];
                        }
                        else
                        {
                            //does the calculations for layers other than 1
                            neuron.Value = 0;
                            for (int np = 0; np < layersOfNN[0][i - 1].Neurons.Count; np++)

                            {
                                neuron.Value = neuron.Value + layersOfNN[0][i - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
                                neuron.Value = sigmoid(neuron.Value + neuron.Bias);
                            }
                        }
                    }
                }
                int index = 0;
                foreach (Neuron item in layersOfNN[0][3].Neurons)
                {
                    outputs[index] = item.Value;
                    index++;
                }
            }
            else
            {
                for (int i = 0; i < layersOfNN[1].Count; i++)
                {
                    Layer lay = layersOfNN[1][i];

                    for (int n = 0; n < lay.NeuronCount; n++)
                    {
                        Neuron neuron = lay.Neurons[n];
                        //sets first layer values to be input
                        if (i == 0)
                        {
                            neuron.Value = inputs[n];
                        }
                        else
                        {
                            //does the calculations for layers other than 1
                            neuron.Value = 0;
                            for (int np = 0; np < layersOfNN[1][i - 1].Neurons.Count; np++)

                            {
                                neuron.Value = neuron.Value + layersOfNN[1][i - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
                                neuron.Value = sigmoid(neuron.Value + neuron.Bias);
                            }
                        }
                    }
                }
                int index = 0;
                foreach (Neuron item in layersOfNN[1][3].Neurons)
                {
                    outputs[index] = item.Value;
                    index++;
                }
            }

            return outputs;
        }
        //called after the game
        public void trainNeuralNetwork(int result, int[] usedBy)
        {
            oldLayers = layers;
            layersOfNN[1] = layers;
            double[] spotSuccess = new double[usedBy.Length];
            if (result != 0)
            {
                for (int i = 0; i < usedBy.Length; i++)
                {
                    if (usedBy[i] == result)
                    {
                        //good, improve weight
                        spotSuccess[i] = 1;

                    }
                    else if (usedBy[i] != 0)
                    {
                        //bad, decrease weights, also makes sure not an unfilled spot
                        spotSuccess[i] = 0;

                    }
                    else
                    {
                        spotSuccess[i] = -1;
                    }
                }
                for (int i = 0; i < layers[layers.Count - 1].Neurons.Count; i++)
                {
                    //finds valuue needs to change by for baddies
                    if (spotSuccess[i] != -1)
                    {
                        Neuron n = layers[layers.Count - 1].Neurons[i];
                        n.Delta = n.Value * (1 - n.Value) * (spotSuccess[i] - n.Value);
                        //goes back layer by layer while excluding output and input
                        for (int j = layers.Count - 2; j > 2; j--)
                        {
                            for (int k = 0; k < layers[j].Neurons.Count; k++)
                            {
                                Neuron n2 = layers[j].Neurons[k];
                                n.Delta = n.Value * (1 - n.Value) * layers[j + 1].Neurons[i].Dendrites[k].Weight * layers[j + 1].Neurons[i].Delta;

                            }
                        }
                    }
                }
                //goes through each layer except input and reassigns values
                for (int i = layers.Count - 1; i >= 1; i--)
                {
                    for (int j = 0; j < layers[i].NeuronCount; j++)
                    {
                        Neuron n = layers[i].Neurons[j];
                        n.Bias = n.Bias + (learningRate * n.Delta);
                        for (int k = 0; k < n.DendriteCount; k++)
                        {
                            n.Dendrites[k].Weight = n.Dendrites[k].Weight + (learningRate * layers[i - 1].Neurons[k].Value * n.Delta);

                        }
                    }
                }
                if (layers == oldLayers)
                {
                    resetOldLayer();
                }
                saveToDatabase();
            }
            else
            {
                resetOldLayer();
            }


        }
        List<Layer> oldLayers = new List<Layer>();

        //saves the neural network after training
        public void saveToDatabase()
        {
            string folderOfProgram = AppDomain.CurrentDomain.BaseDirectory;
            string dir = folderOfProgram.Substring(0, folderOfProgram.Length - 26);
            string serialFile = Path.Combine(dir, "layers.bin");
            string serialFile2 = Path.Combine(dir, "layers2.bin");

            using (Stream stream = File.Open(serialFile2, FileMode.Create))
            {
                var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bFormatter.Serialize(stream, oldLayers);
            }
            using (Stream stream = File.Open(serialFile, FileMode.Create))
            {
                var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bFormatter.Serialize(stream, layers);
            }
        }
        //check if neural network exists and if so gets it.
        //TODO make sure the neurons and dendrites also saved
        public bool getFromDatabase()
        {
            try
            {
                //get from database
                string folderOfProgram = AppDomain.CurrentDomain.BaseDirectory;
                string dir = folderOfProgram.Substring(0, folderOfProgram.Length - 26);
                string serialFile = Path.Combine(dir, "layers.bin");
                string serialFile2 = Path.Combine(dir, "layers2.bin");

                using (Stream stream = File.Open(serialFile, FileMode.Open))
                {
                    var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    layers = (List<Layer>)bFormatter.Deserialize(stream);
                }
                using (Stream stream = File.Open(serialFile2, FileMode.Open))
                {
                    var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    oldLayers = (List<Layer>)bFormatter.Deserialize(stream);
                }
                if (layers == null)
                {
                    return false;
                }



                return true;
            }
            catch (Exception)
            {
                // shit happened
                return false;
            }

        }
        public void resetOldLayer()
        {
            oldLayers = new List<Layer>();
            oldLayers.Add(new Layer(42));
            for (int i = 0; i < 2; i++)
            {
                oldLayers.Add(new Layer(70));
            }
            oldLayers.Add(new Layer(42));
            for (int i = 0; i < oldLayers.Count; i++)
            {
                for (int n = 0; n < oldLayers[i].NeuronsNum; n++)
                {
                    oldLayers[i].Neurons.Add(new Neuron());



                }
                oldLayers[i].Neurons.ForEach((neuron) =>
                {
                    if (i == 0)
                    {
                        //no need to check file because should be no change from time to time that is not fixed above
                        neuron.Bias = 0;
                    }
                    else
                    {
                        for (int d = 0; d < oldLayers[i - 1].NeuronCount; d++)
                        {
                            neuron.Dendrites.Add(new Dendrite());

                        }
                    }
                });
            }
            layersOfNN[1] = oldLayers;
        }
    }
    public class CryptoRandom
    {
        public double RandomValue { get; set; }

        public CryptoRandom()
        {
            using (RNGCryptoServiceProvider p = new RNGCryptoServiceProvider())
            {
                Random r = new Random(p.GetHashCode());
                this.RandomValue = r.NextDouble();
            }
        }
    }
    //the hidden layers, when initizialed are random
    [Serializable()]

    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            CryptoRandom n = new CryptoRandom();
            this.Weight = n.RandomValue;
        }
    }
    [Serializable()]
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        public int NeuronsNum { get; set; }

        public int NeuronCount
        {
            get
            {
                return Neurons.Count;
            }
        }

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
            NeuronsNum = numNeurons;
        }
    }
    [Serializable()]

    public class Neuron
    {
        public List<Dendrite> Dendrites { get; set; }
        public double Bias { get; set; }
        public double Delta { get; set; }
        public double Value { get; set; }

        public int DendriteCount
        {
            get
            {
                return Dendrites.Count;
            }
        }

        public Neuron()
        {
            Random n = new Random(Environment.TickCount);
            this.Bias = n.NextDouble();
            this.Dendrites = new List<Dendrite>();
        }
    }
}

