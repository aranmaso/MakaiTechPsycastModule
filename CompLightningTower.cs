using System;
using System.Linq;
using UnityEngine;
using RimWorld;
using Verse;
using VFECore;
using VanillaPsycastsExpanded;
using System.Collections.Generic;

namespace MakaiTechPsycast.TrueDestruction
{

	public class CompLightningTower : ThingComp
	{
		public CompProperties_LightningTower Props => (CompProperties_LightningTower)props;

		public bool isToggledOn = false;

		public bool isAttackDowned = false;

		public bool laserBeamToggle;

		public bool attackDowned;

		public bool attackDownedToggle = true;
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.canToggleLaserBeam)
			{
				Command_Toggle command_ToggleLink = new Command_Toggle();
				if (isToggledOn)
				{
					command_ToggleLink.defaultLabel = "PowerBeam Enabled";
					command_ToggleLink.defaultDesc = "PowerBeam Enabled";
				}
				else
				{
					command_ToggleLink.defaultLabel = "PowerBeam Disabled";
					command_ToggleLink.defaultDesc = "PowerBeam Disabled";
				}
				command_ToggleLink.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_ToggleLink.icon = ContentFinder<Texture2D>.Get(Props.uiIcon);
				command_ToggleLink.isActive = () => isToggledOn;
				command_ToggleLink.toggleAction = delegate
				{
					isToggledOn = !isToggledOn;
					if (isToggledOn)
					{
						laserBeamToggle = true;
					}
					else
					{
						laserBeamToggle = false;
					}
				};
				yield return command_ToggleLink;
			}
			if (attackDownedToggle == true)
			{
				Command_Toggle command_ToggleLink = new Command_Toggle();
				if (isAttackDowned)
				{
					command_ToggleLink.defaultLabel = "Attack downed pawn enabled";
					command_ToggleLink.defaultDesc = "will target downed pawn";
				}
				else
				{
					command_ToggleLink.defaultLabel = "Attack downed pawn disabled";
					command_ToggleLink.defaultDesc = "will not target downed pawn";
				}
				command_ToggleLink.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_ToggleLink.icon = ContentFinder<Texture2D>.Get(Props.uiIcon);
				command_ToggleLink.isActive = () => isAttackDowned;
				command_ToggleLink.toggleAction = delegate
				{
					isAttackDowned = !isAttackDowned;
					if (isAttackDowned)
					{
						attackDowned = true;
					}
					else
					{
						attackDowned = false;
					}
				};
				yield return command_ToggleLink;
			}
		}
		private int nextTest = 0;

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref nextTest, "nextTest", 0);
			base.PostExposeData();
		}

		public override void PostPostMake()
		{
			nextTest = Find.TickManager.TicksGame + Props.tickRate;
			base.PostPostMake();
		}

		public override void CompTick()
		{
			base.CompTick();
			if (Find.TickManager.TicksGame != nextTest)
			{
				return;
			}
			foreach (Thing item in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, Props.radius, useCenter: true))
			{
				if (!(item is Pawn pawn))
				{
					continue;
				}
				/*parent.TryGetQuality(out var qc);
				if (qc == QualityCategory.Normal)
                {
					
                }*/
				System.Random randWarp = new System.Random();
				int randomWarp = randWarp.Next(-7, 7);
				IntVec3 pawnRand = new IntVec3(randomWarp, 0, randomWarp);
				IntVec3 pawnCurrent = pawn.Position;
				IntVec3 pawnNew = (parent.Position + pawnRand);

				float ValidTarget = Rand.Value;
				if (ValidTarget <= 0.25f)
                {
					if (pawn.HostileTo(Faction.OfPlayer) && !pawn.Downed && attackDowned == false)
					{
						float damRand = Rand.Value;
						if (damRand <= 0.5f && laserBeamToggle == true)
						{
							MakaiTD_PowerBeam orbitalStrike = (MakaiTD_PowerBeam)GenSpawn.Spawn(Props.projectile, pawnNew, pawn.Map);
							orbitalStrike.duration = 60;
							orbitalStrike.instigator = pawn;
							orbitalStrike.StartStrike();
						}
						else
						{
							GenExplosion.DoExplosion(pawn.Position, pawn.Map, 2f, DamageDefOf.EMP, null);
						}
						pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrikeGreen(pawn.Map, pawnCurrent));
						pawn.teleporting = true;
						pawn.ExitMap(allowedToJoinOrCreateCaravan: true, Rot4.Invalid);
						pawn.teleporting = false;
						GenSpawn.Spawn(pawn, pawnNew, parent.Map);
						pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrikeGreen(pawn.Map, pawnNew));

						if (pawn.HostileTo(Faction.OfPlayer) && pawn.health.hediffSet.HasHediff(Props.conduct ?? VPE_DefOf.VPE_UnLucky))
							for (int i = 0; i < 2; i++)
							{
								MakaiTD_PowerBeam orbitalStrike = (MakaiTD_PowerBeam)GenSpawn.Spawn(Props.projectile, pawn.Position, pawn.Map);
								orbitalStrike.duration = 60;
								orbitalStrike.instigator = pawn;
								orbitalStrike.StartStrike();
							}
					}
					if ((pawn.HostileTo(Faction.OfPlayer) || !pawn.Faction.IsPlayer) && (attackDowned == true))
					{
						float damRand = Rand.Value;
						if (damRand <= 0.5f && laserBeamToggle == true)
						{
							MakaiTD_PowerBeam orbitalStrike = (MakaiTD_PowerBeam)GenSpawn.Spawn(Props.projectile, pawn.Position, pawn.Map);
							orbitalStrike.duration = 60;
							orbitalStrike.instigator = pawn;
							orbitalStrike.StartStrike();
						}
						else
						{
							GenExplosion.DoExplosion(pawn.Position, pawn.Map, 2f, DamageDefOf.EMP, null);
						}
						pawn.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrikeGreen(pawn.Map, pawn.Position));

						if (pawn.HostileTo(Faction.OfPlayer) && pawn.health.hediffSet.HasHediff(Props.conduct ?? VPE_DefOf.VPE_UnLucky))
							for (int i = 0; i < 2; i++)
							{
								MakaiTD_PowerBeam orbitalStrike = (MakaiTD_PowerBeam)GenSpawn.Spawn(Props.projectile, pawn.Position, pawn.Map);
								orbitalStrike.duration = 60;
								orbitalStrike.instigator = pawn;
								orbitalStrike.StartStrike();
							}
					}
				}
				
			}
			nextTest += Props.tickRate;
		}
	}
}
