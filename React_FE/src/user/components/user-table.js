import React from "react";
import Table from "../../commons/tables/table";
import { Link } from 'react-router-dom';
import { Button } from 'reactstrap';
import * as API_USERS from "../api/user-api";
import APIResponseErrorMessage from "../../commons/errorhandling/api-response-error-message";


const filters = [
    {
        accessor: 'username',
    }
];

class UserTable extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            tableData: this.props.tableData,
            editedUsers: {}
        };
        this.reloadHandler = this.props.reloadHandler;
        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    handleInputChange(column, value, userId) {
        this.setState(prevState => ({
            editedUsers: {
                ...prevState.editedUsers,
                [userId]: {
                    ...prevState.editedUsers[userId],
                    [column]: value
                }
            }
        }));
    }

    handleUpdate(updatedUser) {
        console.log(updatedUser);
        API_USERS.updateUser(updatedUser.hashid, updatedUser, (result, status, error) => {
            if(result !== null && (status === 200 )) {
                /*this.setState(prevState => ({
                    tableData: prevState.tableData.map(user => 
                        user.hashid === updatedUser.hashid ? updatedUser : user
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
    }

    handleDelete(userhashid) {
        API_USERS.deleteUser(userhashid, (result, status, error) => {
            if(status === 200) {
                /*this.setState(prevState => ({
                    tableData: prevState.tableData.filter(user => user.hashid !== userhashid)
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
                Header: 'Hash ID',
                accessor: 'hashid',
            },
            {
                Header: 'Username',
                accessor: 'username',
                Cell: row => (
                    <input
                        type="text"
                        defaultValue={row.original.username}
                        onChange={e => this.handleInputChange('username', e.target.value, row.original.id)}
                    />
                )
            },
            {
                Header: 'Password',
                accessor: 'password',
                Cell: row => (
                    <input
                        type="text"
                        defaultValue={row.original.password}
                        onChange={e => this.handleInputChange('password', e.target.value, row.original.id)}
                    />
                )
            },
            {
                Header: 'Role',
                accessor: 'userRole',
                Cell: row => <span>{row.value === 0 ? 'User' : 'Admin'}</span> 
            },
            {
                Header: 'LinkedDevices',
                accessor: 'linkedDevices',
                Cell: row => (
                    <Link to={`/devices/${row.original.hashid}`}>LinkedDevices</Link> // Redirect to a route, for example /devices/userId
                )
            },
            {
                Header: 'Update',
                accessor: 'update',
                Cell: row => {
                    const { update, delete: deleteProp, linkedDevices, ...rest } = row.original;
                    const editedData = this.state.editedUsers[row.original.id];
                    const editedUsers = {
                        ...rest,
                        ...(editedData || {})  
                    };
                    return (    
                        <Button color="warning" size="sm" 
                                onClick={() => this.handleUpdate(editedUsers)}>
                            Update
                        </Button>
                    );
                }
            },
            {
                Header: 'Delete',
                accessor: 'delete',
                Cell: row => (
                    <Button color="danger" size="sm" 
                            onClick={() => this.handleDelete(row.original.hashid)}>
                        Delete
                    </Button>
                )
            }
        ];
        return (
            <div>
                <Table
                    data={this.state.tableData}
                    columns={columns}
                    search={filters}
                    pageSize={5}
                />
                {
                    this.state.errorStatus > 0 &&
                    <APIResponseErrorMessage errorStatus={this.state.errorStatus} error={this.state.error}/>
                }
            </div>
        );
    }
}

export default UserTable;
