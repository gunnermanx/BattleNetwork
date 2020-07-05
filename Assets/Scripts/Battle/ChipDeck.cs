using UnityEngine;
using System.Collections.Generic;

namespace BattleNetwork.Battle
{
    public class ChipDeck
    {
        private List<DummyChip> chipDeck = new List<DummyChip>();
        private List<DummyChip> chipHand = new List<DummyChip>();

        private System.Random rng;
       
        private void LoadDeckData()
        {
            rng = new System.Random();

            // "load" some dummy chip data for now
            chipDeck.Add(new DummyChip(0));
            chipDeck.Add(new DummyChip(1));
            chipDeck.Add(new DummyChip(2));
            chipDeck.Add(new DummyChip(3));
            chipDeck.Add(new DummyChip(4));
            chipDeck.Add(new DummyChip(5));
            chipDeck.Add(new DummyChip(6));
            chipDeck.Add(new DummyChip(7));
            chipDeck.Add(new DummyChip(8));
            chipDeck.Add(new DummyChip(9));

            // shuffle it once, this is the order of the deck
            Shuffle();
        }


        public class DummyChip
        {
            int id;
            public DummyChip(int _id)
            {
                id = _id;
            }
        }

        public void DrawChip()
        {
            DummyChip drawnChip = chipDeck[0];
            chipDeck.RemoveAt(0);
            chipHand.Add(drawnChip);
        }

        public void ReturnChipAt(int index)
        {
            DummyChip returnedChip = chipHand[index];
            chipHand.RemoveAt(index);
            chipDeck.Add(returnedChip);
        }


        private void Shuffle()
        {
            int i = chipDeck.Count;
            while ( i > 1 )
            {
                i--;
                int j = rng.Next(i + 1);
                DummyChip tmp = chipDeck[j];
                chipDeck[i] = chipDeck[j];
                chipDeck[j] = tmp;
            }
        }

    }
}
