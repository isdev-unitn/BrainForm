/*** Copyright Disclaimer
@Author: Michele Romani
@Website: http://bromans.github.io
@Email: michele.romani.zaltieri@gmail.com
@Date: 2024-06-01
This code is distributed under the CC License: any use of the code should provide credit to the author and a link to the original source.
Research and non-commercial use is allowed. Any commercial use must be agreed upon with the author.
***/

using System.Collections.Generic;
using UnityEngine;

namespace CortexBenchmark
{
    public class TaskController3D : MonoBehaviour
    {
        [SerializeField]
        private int nClasses;

        [SerializeField]
        public int nTrials;

        [SerializeField]
        [Tooltip("Performance calculator")]
        protected List<PerformanceMetrics> _performanceCalculators;

        [SerializeField]
        private Collider EndTask;

        private int enemyCounter = 0;

        private int currentClassId = 0;

        void Start()
        {
            _performanceCalculators.ForEach(p => p.Initialize(nClasses, nTrials));
        }

        public void TaskTriggerEntered(string triggerName)
        {

            if (triggerName == "StartTask")
            {
                if (enemyCounter == 0)
                {
                    StartTaskTimer();
                }
            }
        }

        public void SetCurrentClassId(int classId)
        {
            currentClassId = classId;
        }

        public void TargetHit(int cueId)
        {
            enemyCounter++;
            _performanceCalculators.ForEach(p => p.AddCorrectlyClassified(cueId, currentClassId));
            if (enemyCounter == nTrials)
                StopTaskTimer();
        }

        public void TargetMiss(int cueId)
        {
            enemyCounter++;
            _performanceCalculators.ForEach(p => p.AddMissClassified(cueId, currentClassId));
            if (enemyCounter == nTrials)
                StopTaskTimer();
        }

        public void StartTaskTimer()
        {
            _performanceCalculators.ForEach(p => p.StartTimer());
        }

        public void StopTaskTimer()
        {
            enemyCounter = 0;
            _performanceCalculators.ForEach(p => p.StopTimer());
            _performanceCalculators.ForEach(calculator => calculator.RetrieveTaskScore());
            _performanceCalculators.ForEach(calculator => calculator.Initialize(nClasses, nTrials));
        }
    }
}