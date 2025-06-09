using UnityEngine;

public interface IOwnedByPlayer
{
    public PlayerInstance OwnerPlayer { get; set; }
}
