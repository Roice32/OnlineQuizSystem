import React, { useState, useEffect, useRef } from 'react';
import { BsPencilSquare, BsTrash, BsPlus } from 'react-icons/bs';
import { IoIosAddCircleOutline } from 'react-icons/io';
import Navbar from '../components/Navbar';

const TagsPage = () => {
    const [tags, setTags] = useState([]);
    const [totalTags, setTotalTags] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);
    const [loading, setLoading] = useState(true);
    const [editingTagId, setEditingTagId] = useState(null);
    const [editedTagName, setEditedTagName] = useState('');
    const [newTagName, setNewTagName] = useState('');
    const [showAddTagInput, setShowAddTagInput] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const editInputRef = useRef(null);

    useEffect(() => {
        fetchTags();
    }, [currentPage, pageSize]);

    const fetchTags = async () => {
        setLoading(true);
        try {
            const response = await fetch(`http://localhost:5276/api/tags?limit=${pageSize}&offset=${(currentPage - 1) * pageSize}`);
            const data = await response.json();
            
            if (!response.ok) {
                if (data.message === "No tags found") {
                    setTags([]);
                    setTotalTags(0);
                } else {
                    throw new Error('Failed to fetch tags');
                }
            } else {
                setTags(data.tags);
                setTotalTags(data.totalTags);
            }
        } catch (error) {
            console.error('Error fetching tags:', error);
        } finally {
            setLoading(false);
        }
    };
    

    const handlePageChange = (page) => {
        setCurrentPage(page);
    };

    const handlePageSizeChange = (event) => {
        setPageSize(Number(event.target.value));
        setCurrentPage(1);
    };

    const handleEditTag = async (id) => {
        if (!editedTagName.trim()) {
            setErrorMessage('Numele tag-ului nu poate fi gol.');
            return;
        }
        try {
            const response = await fetch(`http://localhost:5276/api/tags/${id}`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ name: editedTagName }),
            });
            if (!response.ok) {
                throw new Error('Failed to edit tag');
            }
            fetchTags();
            setEditingTagId(null);
            setEditedTagName('');
            setErrorMessage('');
        } catch (error) {
            console.error('Error editing tag:', error);
            setErrorMessage('A apărut o eroare la modificarea tag-ului.');
        }
    };
    

    const renderTags = () => {
        if (loading) {
            return <p className="text-center mt-4">Se încarcă tag-urile...</p>;
        }

        if (tags.length === 0) {
            return (
                <ul className="mt-4">
                    <li className="flex items-center bg-gray-200 rounded-lg px-3 py-3 text-sm mb-2">
                        {showAddTagInput ? (
                            <>
                                <input
                                    type="text"
                                    value={newTagName}
                                    onChange={(e) => setNewTagName(e.target.value)}
                                    placeholder="Numele noului tag"
                                    className="flex-grow text-xl bg-white border border-gray-300 rounded-lg px-3 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                                <button
                                    className="ml-1 text-green-500 text-2xl"
                                    title="Adaugă tag nou"
                                    onClick={handleAddTag}
                                >
                                    <BsPlus />
                                </button>
                            </>
                        ) : (
                            <button
                                className="text-green-500 text-3xl"
                                title="Adaugă tag nou"
                                onClick={() => setShowAddTagInput(true)}
                            >
                                <IoIosAddCircleOutline />
                            </button>
                        )}
                    </li>
                </ul>
            );
        }

        return (
            <ul className="mt-4">
                {tags.map(tag => (
                    <li key={tag.id} className="flex items-center bg-gray-200 rounded-lg px-3 py-3 text-sm mb-2">
                        {editingTagId === tag.id ? (
                            <div ref={editInputRef} className="flex-grow flex items-center">
                                <input
                                    type="text"
                                    value={editedTagName}
                                    onChange={(e) => setEditedTagName(e.target.value)}
                                    className="flex-grow text-xl bg-white border border-gray-300 rounded-lg px-3 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                                <button
                                    className="ml-1 text-blue-500 text-2xl"
                                    title="Salvează modificările"
                                    onClick={() => handleEditTag(tag.id)}
                                >
                                    <BsPencilSquare />
                                </button>
                            </div>
                        ) : (
                            <>
                                <span className="flex-grow text-xl">{tag.name}</span>
                                <button
                                    className="ml-1 text-blue-500 text-2xl"
                                    title="Modificare"
                                    onClick={() => {
                                        setEditingTagId(tag.id);
                                        setEditedTagName(tag.name);
                                    }}
                                >
                                    <BsPencilSquare />
                                </button>
                                <button
                                    className="ml-1 text-red-500 text-2xl"
                                    title="Ștergere"
                                    onClick={() => handleDeleteTag(tag.id)}
                                >
                                    <BsTrash />
                                </button>
                            </>
                        )}
                    </li>
                ))}
                <li className="flex items-center bg-gray-200 rounded-lg px-3 py-3 text-sm mb-2">
                    {showAddTagInput ? (
                        <>
                            <input
                                type="text"
                                value={newTagName}
                                onChange={(e) => setNewTagName(e.target.value)}
                                placeholder="Numele noului tag"
                                className="flex-grow text-xl bg-white border border-gray-300 rounded-lg px-3 py-1 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <button
                                className="ml-1 text-green-500 text-2xl"
                                title="Adaugă tag nou"
                                onClick={handleAddTag}
                            >
                                <BsPlus />
                            </button>
                        </>
                    ) : (
                        <button
                            className="text-green-500 text-3xl"
                            title="Adaugă tag nou"
                            onClick={() => setShowAddTagInput(true)}
                        >
                            <IoIosAddCircleOutline />
                        </button>
                    )}
                </li>
            </ul>
        );
    };

    const handleDeleteTag = async (id) => {
        try {
           
            const response = await fetch(`http://localhost:5276/api/tags/${id}`, {
                method: 'DELETE',
            });
            if (!response.ok) {
                throw new Error('Failed to delete tag');
            }
            const remainingTags = tags.length - 1;
            if (remainingTags === 0 && currentPage > 1) {
                setCurrentPage(currentPage - 1);
            } else {
                fetchTags();
            }
        } catch (error) {
            console.error('Error deleting tag:', error);
        }
    };
    

    const handleAddTag = async () => {
        if (!newTagName.trim()) {
            setErrorMessage('Numele tag-ului nu poate fi gol.');
            return;
        }
        try {
            const response = await fetch(`http://localhost:5276/api/tags`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ name: newTagName }),
            });
            if (!response.ok) {
                throw new Error('Failed to add tag');
            }
            fetchTags();
            setNewTagName('');
            setShowAddTagInput(false);
            setErrorMessage('');
        } catch (error) {
            console.error('Error adding tag:', error);
            setErrorMessage('A apărut o eroare la adăugarea tag-ului.');
        }
    };
    

    const totalPages = Math.ceil(totalTags / pageSize);

    const handleClickOutside = (event) => {
        if (editInputRef.current && !editInputRef.current.contains(event.target)) {
            cancelEdit();
        }
    };

    const cancelEdit = () => {
        setEditingTagId(null);
        setEditedTagName('');
    };

    useEffect(() => {
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <div>
            <Navbar />
            <div className="max-w-2xl mx-auto px-4">
                <h1 className="text-2xl font-bold mt-8 mb-4 text-center">TAGS</h1>
                {errorMessage && (
                    <p className="text-center text-red-500 mb-4">{errorMessage}</p>
                )}
                {renderTags()}
                <div className="flex justify-center items-center mt-4">
                    <select
                        value={pageSize}
                        onChange={handlePageSizeChange}
                        className="px-2 py-1 border border-gray-300 rounded-md"
                    >
                        <option value={1}>1</option>
                        <option value={5}>5</option>
                        <option value={10}>10</option>
                    </select>
                </div>
                {totalPages > 1 && (
                    <div className="flex justify-center mt-4">
                        {[...Array(totalPages).keys()].map(page => (
                            <button
                                key={page + 1}
                                onClick={() => handlePageChange(page + 1)}
                                className={`bg-blue-500 text-white rounded-lg px-3 py-1 ${currentPage === page + 1 ? 'bg-blue-700' : ''} ml-2`}
                            >
                                {page + 1}
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default TagsPage;
