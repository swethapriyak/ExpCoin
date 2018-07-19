using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            int blockCount = 0;
            int option = 0;
            ConcurrentChain chain = new ConcurrentChain(Network.Main);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            List<Transaction> txList = new List<Transaction>();
            RandomUtils.Random = new UnsecureRandom();
            Key subbuPrivateKey = new Key(); Key swethaPrivateKey = new Key();
            // Key adityaPrivateKey = new Key(); Key kavyaPrivateKey = new Key();
            BitcoinSecret subbu = subbuPrivateKey.GetBitcoinSecret(Network.Main);
            BitcoinSecret swetha = swethaPrivateKey.GetBitcoinSecret(Network.Main);
            //BitcoinSecret aditya = adityaPrivateKey.GetBitcoinSecret(Network.Main);
            //BitcoinSecret kavya = kavyaPrivateKey.GetBitcoinSecret(Network.Main);
            decimal subbuCoin1 = 0;
            decimal subbuCoin2 = 0;
            decimal sendAmount = 0;
            Console.WriteLine("\n Enter first coin value:");
            subbuCoin1 = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("\n Enter second coin value:");
            subbuCoin2 = Convert.ToDecimal(Console.ReadLine());
            decimal totalAmount = subbuCoin1 + subbuCoin2;
            Console.WriteLine("\n Total Coins Value: " + totalAmount);
            while (blockCount != 10)
            {
                while (option != 1)
                {
                    Console.WriteLine("\n Enter Send amount:");
                    sendAmount = Convert.ToDecimal(Console.ReadLine());
                    Transaction subbuFunding = new Transaction()
                    {
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
                    txList.Add(tx);
                    Console.WriteLine("Press 1 for Mine or 2 for next transaction:");
                    option = int.Parse(Console.ReadLine());
                }

                chain.SetTip(CreateBlock(now, blockCount, txList, chain));
                blockCount++;
                option = 0;
            }
            //Console.WriteLine("\n Create genesis block:");
            //Console.ReadLine();
            //Console.WriteLine("Genesis block craeted:"+ExpCoin.Program.GetFirstBitcoinAddressEver(tx));
            //Console.ReadLine();

            //ChainedBlock block =ExpCoin.Program.CreateBlock(DateTimeOffset.Now, 1);
        }

        private static string GetFirstBitcoinAddressEver(Transaction tx)
        {
            var genesisBlock = Network.Main.GetGenesis();
            genesisBlock.Transactions.Add(tx);
            var firstTransactionEver = genesisBlock.Transactions.First();
            var firstOutputEver = firstTransactionEver.Outputs.First();
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;
            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();
            var firstBitcoinAddressEver = firstPubKeyEver.GetAddress(Network.Main);
            return firstBitcoinAddressEver.ToString();
        }

        private static ChainedBlock CreateBlock(DateTimeOffset now, int offset, List<Transaction> txList, ChainBase chain = null)
        {
            Block b = Consensus.Main.ConsensusFactory.CreateBlock();
            if (chain != null)
            {
                b.Header.HashPrevBlock = chain.Tip.HashBlock;
                b.Transactions.AddRange(txList);
                Console.WriteLine("Previous Block Hash" + b.Header.HashPrevBlock);
                Console.WriteLine("Created Block Hash:" + b.GetHash());
                Console.ReadLine();
                return new ChainedBlock(b.Header, null, chain.Tip);
            }
            else
                return new ChainedBlock(b.Header, 0);
        }
    }
}
