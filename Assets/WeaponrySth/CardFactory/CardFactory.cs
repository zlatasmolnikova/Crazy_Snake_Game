using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject cardAvatarPrefab;

    [SerializeField]
    private Texture2D piercingTexture;

    [SerializeField]
    private Texture2D bouncinessIncreaseTexture;

    [SerializeField]
    private Texture2D damageIncreaseConstantTexture;

    [SerializeField]
    private Texture2D gunShotTexture;

    [SerializeField]
    private Texture2D explosionTexture;

    [SerializeField]
    private Texture2D grenadeTexture;

    [SerializeField]
    private Texture2D AscendTreeTexture;

    [SerializeField]
    private Texture2D AscendTreeTwiceTexture;

    [SerializeField]
    private Texture2D TrackerTexture;

    [SerializeField]
    private Texture2D CanonBallTexture;

    public CardAvatar CreateCardAvatar(ICard card, Vector3 position)
    {
        var avatar = Instantiate(cardAvatarPrefab, position, Quaternion.identity)
            .GetComponent<CardAvatar>();
        avatar.SetCard(card);
        return avatar;
    }

    public ICard CreateCard(Spell spell)
    {
        // modifs
        if (spell == Spell.Piercing)
        {
            return new PiercingCard(piercingTexture);
        }
        else if (spell == Spell.BouncinessIncrease)
        {
            return new BouncinessIncreaseCard(bouncinessIncreaseTexture);
        }
        else if (spell == Spell.DamageIncreaseConstant)
        {
            return new DamageIncreaseConstCard(damageIncreaseConstantTexture);
        }

        // projectiles
        if (spell == Spell.GunShot)
        {
            return new GunShotCard(gunShotTexture);
        }
        else if (spell == Spell.Explosion)
        {
            return new ExplosionCard(explosionTexture);
        }
        else if (spell == Spell.Grenade)
        {
            return new GrenadeCard(grenadeTexture);
        }
        else if (spell == Spell.Tracker)
        {
            return new TrackerCard(TrackerTexture);
        }
        else if (spell == Spell.CanonBall)
        {
            return new GenericCard(CanonBallTexture, Spell.CanonBall, "CanonBall", SpellType.Projectile, "shoots a canonball");
        }

        // branching
        if (spell == Spell.AscendTree)
        {
            return new GenericCard(AscendTreeTexture, Spell.AscendTree, "AscendTree",
                SpellType.Branching, "Chain next projectile after parent of previous one. So next and previous projectiles will be emmited simultaneously. (Can be stacked)");
        }
        else if (spell == Spell.AscendTreeTwice)
        {
            return new GenericCard(AscendTreeTwiceTexture, Spell.AscendTreeTwice, nameof(Spell.AscendTreeTwice),
                SpellType.Branching, "Chain next projectile after grand parent of previous one. So next and second previous projectiles will be emmited simultaneously");
        }

        throw new System.Exception($"spell {spell} was not resolved by {nameof(CardFactory)}");
    }
}
