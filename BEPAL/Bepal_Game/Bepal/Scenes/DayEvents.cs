using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bepal;

/// <summary>Fixed door events for the vertical slice (GDD 06 §1).</summary>
public static class DayEvents
{
    public static void OpenDoor(SceneManager m, GameState gs)
    {
        gs.DoorDone = true;
        switch (gs.Day)
        {
            case 2: Toothless(m, gs); break;
            case 3: Merchant(m, gs); break;
            case 4: Storm(m, gs); break;
            default: BigZ(m, gs); break;
        }
    }

    static Action<SpriteBatch, float> Portrait(Action<SpriteBatch, float> draw) => draw;

    // ---------- Day 2 ----------
    static void Toothless(SceneManager m, GameState gs)
    {
        var portrait = Portrait((sb, t) =>
            Art.Pet(sb, Species.Toothless, Pet.Create(Species.Toothless).Color, new Vector2(Gfx.W / 2f, 440), 2.2f, t, Input.Mouse.ToVector2()));
        m.Push(new DialogueScene(new[]
        {
            new Line("", "You open the red door. Something slick and black slides out of the dark."),
            new Line("Toothless", "Arrrrrrhrhrhrhhrrhrhrha"),
            new Line("", "It has no teeth. Its throat sac bubbles with purple acid. It looks at your pets... hungrily? Lonely?"),
        }, () => ToothlessChoice(m, gs), portrait));
    }

    static void ToothlessChoice(SceneManager m, GameState gs)
    {
        m.Push(new ChoiceScene("Toothless is at the door. What will you do?", new[]
        {
            new Option("Chase", () => m.Push(DialogueScene.Say("", null,
                "You wave your arms and shout. Toothless hisses and slinks back into the dark. (No energy spent.)"))),
            new Option("Tame", () => m.Push(new PetPickScene("Choose your pet", gs.Alive, pet =>
                m.Push(new FightScene(gs, Enemy.Toothless(), pet, won =>
                {
                    if (!won)
                    {
                        m.Reset(new GameOverScene("All your pets have fallen to Toothless."));
                        return;
                    }
                    var t = Pet.Create(Species.Toothless);
                    gs.Pets.Add(t);
                    gs.Coin += Balance.ToothlessReward;
                    m.Push(DialogueScene.Say("", null, "You got new pet !!!",
                        $"Toothless curls up by the hearth. (+{Balance.ToothlessReward} coin from the sanctuary fund)"));
                })), onCancel: () => ToothlessChoice(m, gs)))
            , gs.Alive.Any()),
        }));
    }

    // ---------- Day 3 ----------
    static void Merchant(SceneManager m, GameState gs)
    {
        var target = gs.Pets.FirstOrDefault(p => p.Species == Species.Toothless && !p.Dead) ?? gs.Alive.FirstOrDefault() ?? gs.Pets[0];
        var portrait = Portrait((sb, t) => Art.Merchant(sb, new Vector2(Gfx.W / 2f, 490), t));
        m.Push(new DialogueScene(new[]
        {
            new Line("", "A tall figure in a wide hat waits outside, a cart of glass bottles and cages behind him."),
            new Line("Merchant", $"I'm quite interested in your {target.Name}... can you give it to me?"),
            new Line("Merchant", $"I'll pay {Balance.MerchantOffer:N0} coin. More than this little shelter earns in a lifetime."),
        }, () => m.Push(new ChoiceScene($"Sell {target.Name} for {Balance.MerchantOffer:N0} coin?", new[]
        {
            new Option("Sell", () =>
            {
                gs.Pets.Remove(target);
                gs.Coin += Balance.MerchantOffer;
                m.Push(DialogueScene.Say("Merchant", null, "A pleasure doing business. It'll be... well looked after.",
                    $"The cart rolls away. You can still hear {target.Name} crying long after it's gone."));
            }, gs.Pets.Count > 1),
            new Option("Refuse", () => m.Push(DialogueScene.Say("Merchant", () => m.Push(new ShopScene(gs)),
                "Hmph. Sentimental. Suit yourself.", "Then perhaps you'd like to buy something instead?"))),
        })), portrait));
    }

    // ---------- Day 4 ----------
    static void Storm(SceneManager m, GameState gs)
    {
        foreach (var p in gs.Pets)
        {
            p.Clean -= Balance.StormCleanLoss;
            p.ClampStats();
        }
        gs.DisasterSeen = true;
        Gfx.Shake(18, 0.8f);
        m.Push(DialogueScene.Say("", null,
            "You crack the door open. No one is there. Only the wind.",
            "THUNDER STORM! Mud and lightning blast through the sanctuary!",
            $"Every pet got filthy! (Clean -{Balance.StormCleanLoss})",
            "New notebook entry: Disaster - Thunder storm."));
    }

    // ---------- Day 5 ----------
    static void BigZ(SceneManager m, GameState gs)
    {
        var portrait = Portrait((sb, t) => Art.Enemy(sb, Enemy.BigZ(), new Vector2(Gfx.W / 2f, 520), t, Input.Mouse.ToVector2(), 0));
        void End() => m.Reset(new ToBeContinuedScene(gs.Day));
        m.Push(new DialogueScene(new[]
        {
            new Line("", "KNOCK. KNOCK. The whole door bends inward."),
            new Line("Big Z", "So THIS is where the little strays hide."),
            new Line("Big Z", "This planet is MINE now. Everything on it too."),
        }, () =>
        {
            if (!gs.Alive.Any())
            {
                m.Push(DialogueScene.Say("", End, "There is no one left to stand against Big Z."));
                return;
            }
            m.Push(new PetPickScene("Choose your pet", gs.Alive, pet =>
                m.Push(new FightScene(gs, Enemy.BigZ(), pet, _ =>
                    m.Push(DialogueScene.Say("Big Z", End, "Pathetic. Keep your shelter warm for me, keeper."))))));
        }, portrait));
    }
}
