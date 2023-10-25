import React from "react";
import { Link, Button } from "react-router-dom"; // Assuming you're using react-router-dom for the Link
import Table from "../../commons/tables/table";
import { API_DEVICES } from "../../api/device-api"; // Adjust this import path to where your device-api.js is
import { Button } from 'reactstrap';

const columns = [
    {
        Header: 'ID',
        accessor: 'id',
    },
    {
        Header: 'Description',
        accessor: 'description',
    },
    {
        Header: 'Address',
        accessor: 'address',
    },
    {
        Header: 'Max Hourly Energy Consumption',
        accessor: 'maximumHourlyEnergyConsumption',
    },
    {
        Header: 'Update',
        accessor: 'update',
        Cell: row => (
            <Button color="warning" size="sm" 
                    onClick={() => this.handleUpdate(row.original)}>
                Update
            </Button>
        )
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

const filters = [
    {
        accessor: 'description',
    }
];

class DeviceTable extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            tableData: this.props.tableData
        };
        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
    }

    handleUpdate(updatedDevice) {
        API_DEVICES.updateDevice(updatedDevice.id, updatedDevice, (response) => {
            if(response && response.ok) {
                
                this.setState(prevState => ({
                    tableData: prevState.tableData.map(device => 
                        device.id === updatedDevice.id ? updatedDevice : device
                    )
                }));
            } else {
                console.log("Error updating device");
            }
        });
    }

    handleDelete(deviceId) {
        API_DEVICES.deleteDevice(deviceId, (response) => {
            if(response && response.ok) {
                this.setState(prevState => ({
                    tableData: prevState.tableData.filter(device => device.id !== deviceId)
                }));
            } else {
                console.log("Error deleting device");
            }
        });
    }

    render() {
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
