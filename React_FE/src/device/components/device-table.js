import React from "react";
import { Link} from "react-router-dom"; 
import Table from "../../commons/tables/table";
import * as API_DEVICES  from "../api/device-api"; 
import { Button } from 'reactstrap';



const filters = [
    {
        accessor: 'description',
    }
];

class DeviceTable extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            tableData: this.props.tableData,
            editedDevices: {}
        };
        this.reloadHandler = this.props.reloadHandler;
        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }
    
    handleInputChange(column, value, deviceId) {
        this.setState(prevState => ({
            editedDevices: {
                ...prevState.editedDevices,
                [deviceId]: {
                    ...prevState.editedDevices[deviceId],
                    [column]: value
                }
            }
        }));
    }
    

    handleUpdate(updatedDevice) {
        API_DEVICES.updateDevice(this.props.userId, updatedDevice.id, updatedDevice, (result, status, error) => {
            if(result !== null && status === 200) {
                /*this.setState(prevState => ({
                    tableData: prevState.tableData.map(device => 
                        device.id === updatedDevice.id ? updatedDevice : device
                    )
                }));*/
                this.reloadHandler();
            } else {
                this.setState(({
                    errorStatus: status,
                    error: error
                }));
            }
        });
        this.setState(prevState => {
            const newEdits = { ...prevState.editedDevices };
            delete newEdits[updatedDevice.id];
            return { editedDevices: newEdits };
        });
        
    }

    handleDelete(deviceId) {
        API_DEVICES.deleteDevice(this.props.userId,deviceId, (result, status, error) => {
            if(status === 200) {
                /*this.setState(prevState => ({
                    tableData: prevState.tableData.filter(device => device.id !== deviceId)
                }));*/
                this.reloadHandler();
            } else {
                this.setState(({
                    errorStatus: status,
                    error: error
                }));
            }
        });
    }

    render() {
        const columns = [
            {
                Header: 'ID',
                accessor: 'id',
            },
            {
                Header: 'Description',
                accessor: 'description',
                Cell: row => (
                    <input
                        type="text"
                        defaultValue={row.original.description}
                        onChange={e => this.handleInputChange('description', e.target.value, row.original.id)}
                    />
                )
            },
            {
                Header: 'Address',
                accessor: 'address',
                Cell: row => (
                    <input
                        type="text"
                        defaultValue={row.original.address}
                        onChange={e => this.handleInputChange('address', e.target.value, row.original.id)}
                    />
                )
            },
            {
                Header: 'Max Hourly Energy Consumption',
                accessor: 'maximumHourlyEnergyConsumption',
                Cell: row => (
                    <input
                        type="text"
                        defaultValue={row.original.maximumHourlyEnergyConsumption}
                        onChange={e => this.handleInputChange('maximumHourlyEnergyConsumption', e.target.value, row.original.id)}
                    />
                )
            },
            {
                Header: 'Monitor Consumption',
                accessor: 'monitoringData', 
                Cell:  row => (
                    <Link to={`/monitoring-consumption/${row.original.id}`}>View Consumption</Link>
                )
            },
            
            {
                Header: 'Update',
                accessor: 'update',
                Cell: row => {
                    const { update, delete: deleteProp, ...rest } = row.original;
                    const editedData = this.state.editedDevices[row.original.id];
                    const updatedDevice = {
                        ...rest,
                        ...(editedData || {})  
                    };
                    return (
                        <Button color="warning" size="sm" 
                                onClick={() => this.handleUpdate(updatedDevice)}>
                            Update
                        </Button>
                    )
                }
            },
            {
                Header: 'Delete',
                accessor: 'delete',
                Cell: row => (
                    <Button color="danger" size="sm" 
                            onClick={() => this.handleDelete(row.original.id)}>
                        Delete
                    </Button>
                )
            }
        ];
        return (
            <Table
                data={this.state.tableData}
                columns={columns}
                search={filters}
                pageSize={5}
            />
        );
    }
}

export default DeviceTable;
