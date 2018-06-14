using NBitcoin;
using System;
using System.Linq;

namespace ExpCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal sendAmount = 0;
            RandomUtils.Random = new UnsecureRandom();

            Key subbuPrivateKey = new Key(); Key swethaPrivateKey = new Key();
            Key adityaPrivateKey = new Key(); Key kavyaPrivateKey = new Key();

            BitcoinSecret subbu = subbuPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret swetha = swethaPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret aditya = adityaPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret kavya = kavyaPrivateKey.GetBitcoinSecret(Network.Main);

            Console.WriteLine("\n Enter first coin value:");
            subbuCoin1 = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("\n Enter second coin value:");
            subbuCoin2 = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("\n Enter Send amount:");
            sendAmount = Convert.ToDecimal(Console.ReadLine());
            Transaction subbuFunding = new Transaction() {
                Outputs =
    {
        new TxOut(subbuCoin1.ToString(), subbu.GetAddress()),
        new TxOut(subbuCoin2.ToString(), subbu.PubKey)
    }
            };
            Coin[] subbuCoins = subbuFunding
                                    .Outputs
                                    .Select((o, i) => new Coin(new OutPoint(subbuFunding.GetHash(), i), o))
                                    .ToArray();
            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                .AddCoins(subbuCoins)
                .AddKeys(subbu.PrivateKey)
                .Send(swetha.GetAddress(), sendAmount.ToString())
                .SendFees("0.001")
                .SetChange(subbu.GetAddress())
                .BuildTransaction(true);
            Console.WriteLine(tx);
            Console.ReadLine();
        }
    }
}
