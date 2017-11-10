using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using UnityEngine;

public class SupporterBehavior : MonoBehaviour
{
    Node WalletInteract(GameObject participant)
    {
        GameObject hero = GameObject.Find("LeadDaniel");
        IList<Node> movements = new List<Node>();

        return new Sequence(
        TreeUtils.TreeNodeTrace(
            new LeafWait(Val.V(() => (long)UnityEngine.Random.Range(0, 1000))),
            "Interval to pick wallet", false),
        new DecoratorForceStatus(RunStatus.Success,
            TreeUtils.TreeNodeTrace(
            new Sequence(
                TreeUtils.TreeNodeTrace(
                TreeUtils.ActionInteruptFailureWhile(
                   // Go to get wallet        
                   NavToWallet(participant),
                   // Stop if someone else have got it
                   () => IsWalletDropped()
                ),
                "Nav To Wallet", false),
                TreeUtils.TreeNodeTrace(
                GetWallet(participant),
                "Get Wallet", false),

                                // Go to find the leading character
               TreeUtils.TreeNodeTrace(
                participant.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
                    Val.V(() => hero.transform.position), Val.V(2.0f)
                ),
                "Reach To Hero", false),

                // Give wallet
                WalletExchange(participant),
                // Shake hand (guest)
                AcceptHandShaking(participant, hero)
            ), "Wallet Picking Logic", false)
        ),
        TreeUtils.TreeNodeTrace(new LeafWait(1000),
        "Supporter Behavior Gonna End", true)
        );
    }
    Node AnimFriend(GameObject p1, GameObject p2, bool interuptable = false)
    {
        IList<Node> movements = new List<Node>();

        movements.Add(
            new SequenceParallel(
                new DecoratorForceStatus(RunStatus.Success,
                TreeUtils.TreeNodeTrace(
                p1.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
                    Val.V(() => p2.transform.position), 3.0f),
                "Nav to friend", false)),
                new DecoratorForceStatus(RunStatus.Success,
                TreeUtils.TreeNodeTrace(
                p2.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
                    Val.V(() => p1.transform.position), 3.0f),
                "Nav to friend", false))
                    ));
        movements.Add(
            new SequenceParallel(
                p1.GetComponent<BehaviorMecanim>().ST_TurnToFace(
                    Val.V(() => p2.transform.position)),
                p2.GetComponent<BehaviorMecanim>().ST_TurnToFace(
                    Val.V(() => p1.transform.position))
            ));
        if (interuptable)
            movements.Add(
                TreeUtils.ActionInteruptSuccessWhile(
                    new DecoratorLoop(
                    new SelectorShuffle(
                        AnimationPlayer(p1), AnimationPlayer(p2))),
                    () => IsWalletDropped()));
        else
            movements.Add(new DecoratorLoop(
                    new SelectorShuffle(
                        AnimationPlayer(p1), AnimationPlayer(p2))));

        return new Sequence(movements.ToArray());
    }
    Node AnimSingle(GameObject participant, bool interuptable = false)
    {
        if (interuptable)
            return TreeUtils.ActionInteruptSuccessWhile(
                    new DecoratorLoop(AnimationPlayer(participant, true)),
                    () => IsWalletDropped());
        else
            return new DecoratorLoop(AnimationPlayer(participant, true));
    }
    bool IsWalletDropped()
    {
        GameObject wallet = GameObject.Find("Wallet");
        //Debug.Log((wallet.transform.parent == ground.transform).ToString());
        return wallet.transform.parent == null;
    }

    Node AcceptHandShaking(GameObject participant, GameObject hero)
    {
        IList<Node> movements = new List<Node>();
        BodyMecanim bm = participant.GetComponent<BodyMecanim>();
        BodyMecanim hbm = hero.GetComponent<BodyMecanim>();

        movements.Add(TreeUtils.WaitUntilTrue(() => hbm.WaitingHandShake));
        movements.Add(TreeUtils.LeafInvokeIEnum(bm.RightHandReach(hbm.Reflist.hand)));
        movements.Add(new LeafInvoke(() => hbm.WaitingHandShake = false));
        movements.Add(TreeUtils.ActionInteruptSuccessWhile(
            TreeUtils.LeafInvokeIEnum(bm.RightHandKeep(hbm.Reflist.hand)), 
            () => hbm.WaitingHandShake));
        movements.Add(TreeUtils.WaitUntilTrue(() => hbm.WaitingHandShake));
        movements.Add(TreeUtils.LeafInvokeIEnum(bm.RightHandReturn()));
        movements.Add(new LeafInvoke(() => hbm.WaitingHandShake = false));
        movements.Add(new LeafWait(1000));
   
        return new Sequence(movements.ToArray());
    }
    Node WalletExchange(GameObject participant)
    {
        GameObject wallet = GameObject.Find("Wallet");
        IEnumerator<RunStatus> releaseE = participant.GetComponent<BodyMecanim>().RightHandReach();

        return new Sequence(
            TreeUtils.LeafInvokeIEnum(participant.GetComponent<BodyMecanim>().RightHandReach()),
            new LeafInvoke(() => participant.GetComponent<BodyMecanim>().RightHandRelease(wallet)),
            TreeUtils.WaitUntilTrue(()=>!IsWalletDropped()),
            new LeafWait(1000),
            TreeUtils.LeafInvokeIEnum(participant.GetComponent<BodyMecanim>().RightHandReturn())
            );
    }
    Node NavToWallet(GameObject participant)
    {
        GameObject wallet = GameObject.Find("Wallet");

        return TreeUtils.TreeNodeTrace(
            participant.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
            Val.V(() => wallet.transform.position), Val.V(1f)),
            "Nav to wallet", false);
    }
    Node GetWallet(GameObject participant)
    {
        GameObject wallet = GameObject.Find("Wallet");
        Node checkClaim = new LeafAssert(() => !wallet.GetComponent<WalletStatus>().isClaimed);
        Node claimWallet = new LeafInvoke(() => wallet.GetComponent<WalletStatus>().isClaimed = true);

        Node anim = TreeUtils.TreeNodeTrace(
            participant.GetComponent<BehaviorMecanim>().Node_BodyAnimation("PICKUPRIGHT", true), // Not the log output!
            "anim", false);


        Node pickUpWallet = TreeUtils.TreeNodeTrace(
            TreeUtils.LeafInvokeIEnum(participant.GetComponent<BodyMecanim>().RightHandPick(wallet)),
            "pick Up Wallet", true);
        Node removeClaim = new LeafInvoke(() => wallet.GetComponent<WalletStatus>().isClaimed = false);

        return new Sequence(checkClaim, claimWallet, anim, pickUpWallet, removeClaim);
    }
    Node AnimationPlayer(GameObject particapant, bool single = false)
    {
        IList<Node> animList = new List<Node>();
        BehaviorMecanim bm = particapant.GetComponent<BehaviorMecanim>();

        Val<long> duration = Val.V(() => (long)UnityEngine.Random.Range(1000, 5000));
        System.Action<Node> PlayAnim = anim =>
            animList.Add(new Sequence(anim, new LeafWait(duration)));


        PlayAnim(bm.ST_PlayFaceGesture(Val.V("SAD"), Val.V((long)1000)));
        PlayAnim(bm.ST_PlayFaceGesture(Val.V("ROAR"), Val.V((long)1000)));
        PlayAnim(bm.ST_PlayFaceGesture(Val.V("LOOKAWAY"), Val.V((long)1000)));
        PlayAnim(bm.ST_PlayFaceGesture(Val.V("HEADSHAKE"), Val.V((long)1000)));
        PlayAnim(bm.ST_PlayHandGesture(Val.V("YAWN"), Val.V((long)1000)));

        if (!single)
        {
            PlayAnim(bm.ST_PlayBodyGesture(Val.V("TALKING ON PHONE"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayBodyGesture(Val.V("FIGHT"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("WAVE"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("CLAP"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("SHOCK"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("CHEER"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("POINTING"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("LOOKUP"), Val.V((long)1000)));
            PlayAnim(bm.ST_PlayHandGesture(Val.V("BEINGCOCKY"), Val.V((long)1000)));

        }

        return new SelectorShuffle(animList.ToArray());
    }

    public Node BehaviorSingle(GameObject p)
    {
        return new Sequence(
            AnimSingle(p, true),
            WalletInteract(p),
            AnimSingle(p)
            );
    }
    public Node BehaviorFriend(GameObject p1, GameObject p2)
    {
        return new Sequence(
            AnimFriend(p1, p2, true),
            new SequenceParallel(
                WalletInteract(p1),
                WalletInteract(p2)),
            AnimFriend(p1, p2)
            );
    }
}

