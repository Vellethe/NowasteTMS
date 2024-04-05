import React from 'react'
import AgentsTable from '../../components/table/AgentsTable'

const Agents = () => {
  return (
    <>
    <div className="m-7">
      <div className="text-3xl text-center mt-6 text-dark-green">Agents</div>

      <div className=" m-3 text-xl flex justify-end  text-medium-green">
        
        <div className="flex gap-3">
        </div>
      </div>
      <AgentsTable/>
    </div>
    </>
  );
}

export default Agents