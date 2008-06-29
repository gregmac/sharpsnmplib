﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lextm.SharpSnmpLib.Mib
{
	/// <summary>
	/// Definition class.
	/// </summary>
	public sealed class Definition
	{
		uint[] _id;
		string _name;
		string _module;
		int _value;
		IDictionary<int, Definition> _children = new Dictionary<int, Definition>();

		Definition() {} 
		/// <summary>
		/// Creates a <see cref="Definition"/> instance.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="module"></param>
		/// <param name="value"></param>
		[CLSCompliant(false)]
		public Definition(uint[] id, string name, string module, int value)
		{
			_id = id;
			_name = name;
			_module = module;
			_value = value;
		}
		
		/// <summary>
		/// Value.
		/// </summary>
		public int Value
		{
			get
			{
				return _value;
			}
		}
		
		/// <summary>
		/// Children definitions.
		/// </summary>
		public IEnumerable<Definition> Children
		{
			get
			{
				return _children.Values;
			}
		}

		internal static Definition RootDefinition
		{
			get
			{
				return new Definition();
			}
		}
		
		/// <summary>
		/// Returns the textual form.
		/// </summary>
		public string TextualForm
		{
			get
			{
				return _module + "::" + _name;
			}
		}

		/// <summary>
		/// Indexer.
		/// </summary>
		public Definition this[int index]
		{
			get
			{
				foreach (Definition d in _children.Values)
				{
					if (d.GetNumericalForm()[d.GetNumericalForm().Length - 1] == index)
                    {
                        return d;
                    }					
				}
                throw new ArgumentOutOfRangeException("index");
			}
		}

		/// <summary>
		/// Module name.
		/// </summary>
		public string Module
		{
			get
			{
				return _module;
			}
		}
		
		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		/// <summary>
		/// Gets the numerical form.
		/// </summary>
		/// <returns></returns>
		[CLSCompliant(false)]
		public uint[] GetNumericalForm()
		{
			return (uint[])_id.Clone();
		}
		/// <summary>
		/// Add an <see cref="IEntity"/> node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public Definition Add(IEntity node)
		{
			if (_name == node.Parent) {
                Definition def = ToDefinition(node, this);
                Add(def);
				return def;
			}
			foreach (Definition d in _children.Values)
			{
                Definition result = d.Add(node);
				if (result != null) {
					return result;
				}
			}
			return null;
		}
		/// <summary>
		/// Adds a <see cref="Definition"/> child to this <see cref="Definition"/>.
		/// </summary>
		/// <param name="def"></param>
        public void Add(Definition def)
        {
        	if (!_children.ContainsKey(def.Value))
        	{
        		_children.Add(def.Value, def);
        	}
        }

        internal static Definition ToDefinition(IEntity entity, Definition parent)
        {
        	uint[] id =
            	(parent == null || parent.GetNumericalForm() == null || parent.GetNumericalForm().Length == 0) ?
            	null : parent.GetNumericalForm();
        	return new Definition(AppendTo(id, (uint)entity.Value), entity.Name, entity.Module, entity.Value);
        }
        
        internal static uint[] AppendTo(uint[] parentId, uint value)
        {
        	List<uint> n = parentId == null? new List<uint>() : new List<uint>(parentId);
			n.Add(value);
			return n.ToArray();
        }
	}
}