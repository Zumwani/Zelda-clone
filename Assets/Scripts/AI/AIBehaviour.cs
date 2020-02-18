using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Enemies:
//tektite:  jump -> standStill ->
//octorok:  walk & attack if player is in front (at any distance)
//peahat:   flying (invincible) -> land (invincible) ->
//zora:     submerge (invicible) -> surface -> fireball ->
//moblin:   same as octorok?
//leever:   surface (if blue teleport in front of link) -> move towards link for distance -> submerge -> move random (invincible)
//armos:    idle (until link touches it) -> move forward ->
//lynel:    same as octorok?
//ghini:    move random (flying)

//Actions:
//Move random (int max distance, animationCurve path)
//Move forward
//Move towards player (int max distance)
//Change direction
//Idle
//Projectile (enum forward / player)
//Teleport to player (vector2int offset)

//Conditions
//Player in front (int)
//Player move too close or attacked (float distance)
//Player in range (float)
//Can move forward

[CreateAssetMenu]
public class AIBehaviour : ScriptableObject
{

    public AIActionCollection actions;

    public AIComponent Create(Type type)
    {

        if (!IsValid(type))
            throw new ArgumentException("Type does not derive from AIComponent.");

        var path = AssetDatabase.GetAssetPath(this);
        var obj = (AIComponent)CreateInstance(type);

        AssetDatabase.AddObjectToAsset(obj, path);
        obj.name = type.Name;
        
        return obj;

    }

    public void Remove(AIComponent component)
    {
        foreach (var c in FindAllComponents(component))
            AssetDatabase.RemoveObjectFromAsset(c);
    }

    List<AIComponent> FindAllComponents(AIComponent component)
    {

        var l = new List<AIComponent>();
        l.Add(component);

        if (component is AIConditional conditional)
        {
            foreach (var c in conditional.ifFalse)
                l.AddRange(FindAllComponents(c));
            foreach (var c in conditional.ifTrue)
                l.AddRange(FindAllComponents(c));
        }

        return l;

    }

    bool IsValid(Type type) =>
        typeof(AIComponent).IsAssignableFrom(type) && type != typeof(AIComponent);

}
