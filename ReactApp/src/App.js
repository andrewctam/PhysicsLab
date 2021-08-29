import React from 'react';
import options from "./assets/options.png"
import lab from "./assets/lab.png"
import save from "./assets/save.png"
import graph from "./assets/graph.png"


class App extends React.Component {
    render() {
        return (
        <div>

        <Row 
            left = {
                <div>
                    <h1>Interactive Physics Lab</h1> 
                    <div className = "text-center"><a type="button" class="btn btn-danger btn-lg" href = "https://andrewtam.org/PhysicsLab/app/">Launch Lab</a></div>
                </div>
            }
            
            right = { 
                <img className = "img-fluid rounded" src = {lab} />

           }
        />


        <Row 
            left = {<img className = "img-fluid rounded" src = {options} />}
            
            right = { 
            <div>
                <h2 className = "text-center">{"Flexible Parameters"}</h2>
                <p>Variables can be changed in the lab, such as gravity and the coefficient of friction. 
                    &nbsp; In addition, all objects can have their initial vectors of position, velocity, and acceleration changed to their desired value. 
                </p>
             </div>
           }
        />

        <Row 
            left = {
            <div>
                <h2 className = "text-center">{"Graph Raw Data From Your Lab"}</h2>
                <p>This lab has a built in graph. You can have the lab graph raw data from your objects, ranging from time to position to energy. 
                Furthermore, you can export your data to a .csv file to view the data in a spreadsheet.
                </p>
            </div>}
            
           right = { <img className = "img-fluid rounded" src = {graph} />
        
           }
        />

        <Row 
            left = { <img className = "img-fluid rounded" src = {save} /> }
            
            right = {
            <div>
                <h2 className = "text-center">{"Save and Share"}</h2>
                <p>Save your lab and easily re-access it again by opening a generated URL like <br />
                <code>andrewtam.org/PhysicsLab/app?s9SzMDAzM7UyBEMDPVMgNgISQKgLo+rqDKz8k7JSk0u0DayMgcqMrXSNQKqRoCEYG0JEEeoNoYK6hsSpNwILGUMVEFRuDBEEutoI4W7seuqMYZpgnjUCacKpHgA=</code> <br/>
                For larger labs, such as with 100+ objects or 600+ graphed points, you can save your lab to a .txt file.</p>
            </div>}
        />        

        <div class="footer text-center"><a href = "https://github.com/tamandrew/PhysicsLab">GitHub Project Repository</a></div>
    
        </div>)

        
    }
}


class Row extends React.Component {
    render() {
        return ( 
        <div className = "row">
            <div className = "col-sm-6">
                {this.props.left}
            </div>
            <div className = "col-sm-6">
                {this.props.right}
                <img className = "img-fluid rounded" src = {this.props.img} />
            </div>
        </div>
        );
    }
}
export default App;
