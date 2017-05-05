using System.Collections.Generic;
using System.Threading;
using System;

namespace RVO
{
    public class Simulator
    {
		public static bool Threaded = false; // Can't profile/debug threaded version

		private static Simulator instance_ = new Simulator();

        private float time_;
        internal IList<RVOAgent> agents_;
//        internal IList<Obstacle> obstacles_;
        internal KdTree kdTree_;
        internal float timeStep_;

        private int _numWorkers = 8;
        private Worker[] _workers;
        private ManualResetEvent[] _doneEvents;

        public static Simulator Instance { get { return instance_; } }
        private Simulator() { Clear(); }

		public void Clear()
        {
            agents_ = new List<RVOAgent>();
//            obstacles_ = new List<Obstacle>();
            time_ = 0;
            kdTree_ = new KdTree();
            timeStep_ = .1f;

            SetNumWorkers(0);
        }

        public int GetNumWorkers()
        {
            return _numWorkers;
        }

        public void SetNumWorkers(int numWorkers)
        {
            _numWorkers = numWorkers;
            if (_numWorkers <= 0)
            {
                int completionPorts;
                ThreadPool.GetMinThreads(out _numWorkers, out completionPorts);
            }
            _workers = null;
        }

        public float getGlobalTime() { return time_; }
        public int getNumAgents() { return agents_.Count; }
        public float getTimeStep() { return timeStep_; }

        public int addAgent(RVOAgent agent)
        {
            agents_.Add(agent);

            return agents_.Count - 1;
        }

		public void removeAgent(RVOAgent agent)
		{
			// ******** BUG: This will shift your indices and break agentId!! ********
			agents_.Remove(agent);
		}

		public RVOAgent getAgent(int agentId)
		{
			return agents_[agentId];
		}

		public void setTimeStep(float timeStep)
		{
			timeStep_ = timeStep;
		}

        private class Worker
        {
            int _start;
            int _end;
            ManualResetEvent _doneEvent;

            internal Worker(int start, int end, ManualResetEvent doneEvent)
            {
                _start = start;
                _end = end;
                _doneEvent = doneEvent;
            }
            internal void step(object o)
            {
                for (int i = _start; i < _end; ++i)
                {
                    Simulator.Instance.agents_[i].computeNeighbors();
                    Simulator.Instance.agents_[i].computeTargetVelocity();
                }
                _doneEvent.Set();
            }
        }

        public float doStep()
        {
			if (Threaded)
				return doStepThreaded();
			return doStepNotThreaded();
		}

		private float doStepNotThreaded()
		{
			kdTree_.buildAgentTree();

			for (int i = 0; i < agents_.Count; ++i)
			{
				Simulator.Instance.agents_[i].computeNeighbors();
				Simulator.Instance.agents_[i].computeTargetVelocity();
			}

			time_ += timeStep_;
			return time_;
		}
		
		private float doStepThreaded()
		{
            if(_workers == null)
            {
                _workers = new Worker[_numWorkers];
                _doneEvents = new ManualResetEvent[_workers.Length];
                for (int block = 0; block < _workers.Length; ++block)
                {
                    _doneEvents[block] = new ManualResetEvent(false);
                    _workers[block] = new Worker(block * getNumAgents() / _workers.Length, (block + 1) * getNumAgents() / _workers.Length, _doneEvents[block]);
                }
            }

            kdTree_.buildAgentTree();

            for (int block = 0; block < _workers.Length; ++block)
            {
                _doneEvents[block].Reset();
                ThreadPool.QueueUserWorkItem(_workers[block].step);
            }
            WaitHandle.WaitAll(_doneEvents);

            time_ += timeStep_;
            return time_;
        }
    }
}
